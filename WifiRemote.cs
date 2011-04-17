using System;
using System.Linq;
using System.Text;

using MediaPortal.GUI.Library;
using ZeroconfService;
using MediaPortal.Configuration;
using MediaPortal.Player;
using MediaPortal.Util;
using System.Net.NetworkInformation;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace WifiRemote
{
    /// <summary>
    /// WifiRemote is a process plugin that does the following:
    ///     * Publish a bonjour service with the network interface hardware 
    ///       address as txtInfo
    ///     * Accepts tcp socket connections
    ///     * Sends status information about the current MediaPortal instance 
    ///       to all connected clients
    ///     * Receives messages from clients and forwards them to MediaPortal 
    ///       (commands)
    ///     
    /// Google Code project page:
    ///     * http://code.google.com/p/wifiremote/
    ///       Please have a look at our wiki if you want to write a tcp client
    ///     
    /// Project contributors:
    ///     * Shukuyen
    ///     * DieBagger
    ///     
    /// 
    /// NOTE:
    /// You can specify which version of MediaPortal to compile this plugin for
    /// by defining some conditional compilation symbols (in project build 
    /// properties):
    ///     * COMPILE_FOR_1_1_1 - Compiles for MediaPortal 1.1.1 (without
    ///                           plugin compatibility check and MyVideos
    ///                           now playing message)
    ///                           
    ///     * COMPILE_FOR_1_1_2 - Compile for MediaPortal 1.1.2 (without
    ///                           plugin compatibility check)
    ///                           
    ///     * COMPILE_FOR_1_2_0 - Compile for MediaPortal 1.2.0 or later
    ///
    /// 
    /// </summary>
    public class WifiRemote: ISetupForm, IPlugin
    {
        public const string PLUGIN_NAME = "WifiRemote";
        public const string LOG_PREFIX = "[WIFI_REMOTE] ";
        public const int DEFAULT_PORT = 8017;
        public const int SERVER_VERSION = 1;
        private const int UPDATE_INTERVAL = 1000;

        /// <summary>
        /// Server handling communication with the clients
        /// </summary>
        SocketServer socketServer = null;

        /// <summary>
        /// The Bonjour service publish object
        /// </summary>
        NetService publishService = null;
        
        /// <summary>
        /// Bonjour service name (your hostname)
        /// </summary>
        private string serviceName;

        /// <summary>
        /// Bonjour service type
        /// </summary>
        private string serviceType = "_mepo-remote._tcp";

        /// <summary>
        /// Bonjour domain (empty = whole network)
        /// </summary>
        private string domain = "";

        /// <summary>
        /// Service port
        /// </summary>
        private UInt16 port;

        /// <summary>
        /// <code>true</code> if the service is advertised via Bonjour
        /// </summary>
        private bool servicePublished = false;

        /// <summary>
        /// <code>true</code> to not publish the bonjour service
        /// </summary>
        private bool disableBonjour = false;

        /// <summary>
        /// <code>true</code> if the tv plugin is installed
        /// </summary>
        private bool isTvEnabled = false;

        /// <summary>
        /// Thread for sending now playing updates
        /// </summary>
        private Thread nowPlayingUpdateThread;

        /// <summary>
        /// Indicator if now playing update thread is running
        /// </summary>
        private bool nowPlayingUpdateThreadRunning;

        /// <summary>
        /// Mediaportal log type
        /// </summary>
        public enum LogType 
        {
            Debug,
            Info,
            Warn,
            Error
        }

       

        #region ISetupForm Member

        public string PluginName()
        {
            return PLUGIN_NAME;
        }

        public string Description()
        {
            return "A server for remotes via WiFi, for example the iPhone Remote app.";
        }

        public string Author()
        {
            return "Shukuyen <shukuyen@stalk-me.net>";
        }

        public void ShowPlugin()
        {
            SetupForm setup = new SetupForm();
            setup.ShowDialog();
        }

        public bool CanEnable()
        {
            return true;
        }

        public int GetWindowId()
        {
            return -1;
        }

        public bool DefaultEnabled()
        {
            return true;
        }

        public bool HasSetup()
        {
            return true;
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = null;
            strButtonImage = null;
            strButtonImageFocus = null;
            strPictureImage = null;
            return false;
        }

        #endregion

        #region IPlugin Member
        /// <summary>
        /// Plugin started
        /// </summary>
        public void Start()
        {
            // Check if TV plugin is installed
            isTvEnabled = IsAssemblyAvailable("TvControl", null);
            

            Log.Debug(String.Format("{0} Started!", LOG_PREFIX));
            // register event handlers
            GUIWindowManager.OnNewAction += new OnActionHandler(GUIWindowManager_OnNewAction);
            GUIPropertyManager.OnPropertyChanged += new GUIPropertyManager.OnPropertyChangedHandler(GUIPropertyManager_OnPropertyChanged);
            MediaPortal.Util.Utils.OnStartExternal += new Utils.UtilEventHandler(Utils_OnStartExternal);
            MediaPortal.Util.Utils.OnStopExternal += new Utils.UtilEventHandler(Utils_OnStopExternal);
            g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
            g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);
            g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
            g_Player.PlayBackChanged += new g_Player.ChangedHandler(g_Player_PlayBackChanged);
            GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_Receivers);

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);

            

            // Load port from config
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                port = (UInt16)reader.GetValueAsInt(PLUGIN_NAME, "port", DEFAULT_PORT);
                disableBonjour = reader.GetValueAsBool(PLUGIN_NAME, "disableBonjour", false);
                serviceName = reader.GetValueAsString(PLUGIN_NAME, "serviceName", "");
            }

            // Start listening for client connections
            socketServer = new SocketServer(port);
            socketServer.Start();

            // Publish the service via bonjour to the network
            if (!disableBonjour)
            {
                PublishBonjourService();
            }
        }

        /// <summary>
        /// System power mode has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            // Resume from standby
            if (e.Mode == Microsoft.Win32.PowerModes.Resume)
            {
                WifiRemote.LogMessage("Resuming WifiRemote, starting server", LogType.Debug);

                // Restart the socket server
                socketServer.Start();

                // Restart bonjour service
                if (!disableBonjour)
                {
                    PublishBonjourService();
                }
            }
            // Going to standby
            else if (e.Mode == Microsoft.Win32.PowerModes.Suspend)
            {
                WifiRemote.LogMessage("Suspending WifiRemote, stopping server", LogType.Debug);

                // Stop bonjour service
                if (!disableBonjour)
                {
                    publishService.Stop();
                }

                // Stop socket server
                socketServer.Stop();
            }
        }

        /// <summary>
        /// Network status has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                WifiRemote.LogMessage("Network connected, starting server", LogType.Debug);

                // Restart the socket server
                socketServer.Start();

                // Restart bonjour service
                if (!disableBonjour)
                {
                    PublishBonjourService();
                }
            }
            else
            {
                WifiRemote.LogMessage("Network lost, stopping server", LogType.Debug);

                // Stop bonjour service
                if (!disableBonjour)
                {
                    publishService.Stop();
                }

                // Stop socket server
                socketServer.Stop();
            }
        }

        /// <summary>
        /// Event handler when a GUI property was changed
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="tagValue"></param>
        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (tag.Equals("#selecteditem") ||
                tag.Equals("#selecteditem2") ||
                tag.Equals("#highlightedbutton") ||
                tag.Equals("#currentmodule"))
            {
                SendStatus();
            }
            else if (tag.StartsWith("#Play.") ||
                     tag.StartsWith("#TV."))
            {
                socketServer.SendPropertyToClient(tag, tagValue);
            }
        }

        // An action was executed on the GUI
        void GUIWindowManager_OnNewAction(MediaPortal.GUI.Library.Action action)
        {
            switch (action.wID)
            {
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PLAY:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_PAUSE:
                    SendStatus();
                    break;
            }
        }

        /// <summary>
        /// Received a GUIMessage from MediaPortal
        /// </summary>
        /// <param name="message">The message</param>
        void GUIWindowManager_Receivers(GUIMessage message)
        {
            if (message.Message == GUIMessage.MessageType.GUI_MSG_AUDIOVOLUME_CHANGED)
            {
                socketServer.SendVolumeToAllClients();
            }
        }

        /// <summary>
        /// Mediaportal playback changed
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stoptime"></param>
        /// <param name="filename"></param>
        void g_Player_PlayBackChanged(g_Player.MediaType type, int stoptime, string filename)
        {
            // Change media info
            SendStatus();
        }

        /// <summary>
        /// Mediaportal playback stopped
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stoptime"></param>
        /// <param name="filename"></param>
        void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            LogMessage("Playback stopped!", LogType.Debug);
            SendStatus();
            StopNowPlayingUpdateThread();
        }

        /// <summary>
        /// Mediaportal playback ended
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            LogMessage("Playback ended!", LogType.Debug);
            SendStatus();
            StopNowPlayingUpdateThread();
        }

        /// <summary>
        /// Mediaportal playback started
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
        {
            LogMessage("Playback started!", LogType.Debug);
            SendStatus();
            socketServer.SendNowPlayingToAllClients();
            StartNowPlayingUpdateThread();
        }

        /// <summary>
        /// External player stopped playback
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="waitForExit"></param>
        void Utils_OnStopExternal(System.Diagnostics.Process proc, bool waitForExit)
        {
            SendStatus();
        }

        /// <summary>
        /// External player started playback
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="waitForExit"></param>
        void Utils_OnStartExternal(System.Diagnostics.Process proc, bool waitForExit)
        {
            SendStatus();
        }

        /// <summary>
        /// Send the player status to all clients.
        /// </summary>
        public void SendStatus()
        {
            socketServer.SendStatusToAllClientsIfChanged();
        }

        /// <summary>
        /// Plugin stopped
        /// </summary>
        public void Stop()
        {
            // Stop the socket server
            if (socketServer != null)
            {
                socketServer.Stop();
                socketServer = null;
            }

            // Stop the service if it is running
            if (servicePublished)
            {
                publishService.Stop();
                publishService = null;
            }
        }

        /// <summary>
        /// Publish the service via Bonjour protocol to the network
        /// </summary>
        private void PublishBonjourService()
        {
            // Test if Bonjour is installed
            try
            {
                //float bonjourVersion = NetService.GetVersion();
                Version bonjourVersion = NetService.DaemonVersion;
                Log.Info(String.Format("{0} Bonjour version {1} found.", LOG_PREFIX, bonjourVersion.ToString()));
            }
            catch
            {
                Log.Error(String.Format("{0} This plugin needs Bonjour installed! Get it at http://support.apple.com/downloads/Bonjour_for_Windows", LOG_PREFIX));
                Stop();
            }

            publishService = new NetService(domain, serviceType, serviceName, port);

            // Get the MAC addresses and set it as bonjour txt record
            // Needed by the clients to implement wake on lan
            Hashtable dict = new Hashtable();
            StringBuilder hardwareAddresses = new StringBuilder();
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        String hardwareAddress = adapter.GetPhysicalAddress().ToString();
                        if (!hardwareAddress.Equals(String.Empty) && hardwareAddress.Length == 12)
                        {
                            if (hardwareAddresses.Length > 0)
                            {
                                hardwareAddresses.Append(";");
                            }

                            hardwareAddresses.Append(hardwareAddress);
                        }
                    }
                }
            }
            catch (NetworkInformationException e)
            {
                LogMessage("Could not get hardware address: " + e.Message, LogType.Error);
            }
            dict.Add("hwAddr", hardwareAddresses.ToString());
            //publishService.setTXTRecordData(NetService.DataFromTXTRecordDictionary(dict));
            publishService.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
            publishService.DidPublishService += new NetService.ServicePublished(publishService_DidPublishService);
            publishService.DidNotPublishService += new NetService.ServiceNotPublished(publishService_DidNotPublishService);

            publishService.Publish();
        }

        /// <summary>
        /// Service couldn't be published
        /// </summary>
        /// <param name="service"></param>
        /// <param name="exception"></param>
        void publishService_DidNotPublishService(NetService service, DNSServiceException exception)
        {
            Log.Error(String.Format("{0} Bonjour publish error: {1}", LOG_PREFIX, exception.Message));
        }

        /// <summary>
        /// Service was published
        /// </summary>
        /// <param name="service"></param>
        void publishService_DidPublishService(NetService service)
        {
            Log.Info(String.Format("{0} Published Service via Bonjour!", LOG_PREFIX));
            servicePublished = true;
        }

        /// <summary>
        /// Log a message to the mediaportal log
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="type">Type of log (debug, info, error ...)</param>
        public static void LogMessage(string message, LogType type)
        {
            switch (type)
            {
                case LogType.Debug:
                    Log.Debug(String.Format("{0} {1}", LOG_PREFIX, message));
                    break;

                case LogType.Info:
                    Log.Info(String.Format("{0} {1}", LOG_PREFIX, message));
                    break;

                case LogType.Warn:
                    Log.Warn(String.Format("{0} {1}", LOG_PREFIX, message));
                    break;

                case LogType.Error:
                    Log.Error(String.Format("{0} {1}", LOG_PREFIX, message));
                    break;
            }
        }


        /// <summary>
        /// Get the machine name or a fallback
        /// </summary>
        /// <returns></returns>
        public static string GetServiceName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch (InvalidOperationException)
            {
                return "MediaPortal Wifi Remote";
            }
        }


        /// <summary>
        /// Returns an image as its byte array representation.
        /// Used to make images encodable in JSON.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static byte[] imageToByteArray(Image img, System.Drawing.Imaging.ImageFormat format)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, format);
                stream.Close();
                byteArray = stream.ToArray();
            }

            return byteArray;
        }

        #endregion

        /// <summary>
        /// Get all active window plugins and the corresponding window IDs.
        /// This can be used in the client to jump to a specific plugin.
        /// 
        /// We are also sending the plugin icon as byte array if it exists.
        /// </summary>
        internal static ArrayList GetActiveWindowPluginsAndIDs(bool sendIcons)
        {
            ArrayList plugins = new ArrayList();
            int[] ignoredPluginIds = new int[] { 
                -1, 
                0,          // home
                3005,       // GUITopbar
                730716      // fanart handler
            }; 

            foreach (ISetupForm plugin in PluginManager.SetupForms)
            {
                if (!ignoredPluginIds.Contains<int>(plugin.GetWindowId()))
                {
                    if (sendIcons)
                    {
                        byte[] iconBytes = new byte[0];

                        // Load plugin icon
                        Type pluginType = plugin.GetType();
                        PluginIconsAttribute[] icons = (PluginIconsAttribute[])pluginType.GetCustomAttributes(typeof(PluginIconsAttribute), false);
                        if (icons.Length > 0)
                        {
                            string resourceName = icons[0].ActivatedResourceName;
                            if (!string.IsNullOrEmpty(resourceName))
                            {
                                System.Drawing.Image icon = null;
                                try
                                {
                                    icon = System.Drawing.Image.FromStream(pluginType.Assembly.GetManifestResourceStream(resourceName));
                                }
                                catch (Exception e)
                                {
                                    WifiRemote.LogMessage("Could not load plugin icon: " + e.Message, WifiRemote.LogType.Error);
                                }

                                if (icon != null)
                                {
                                    iconBytes = WifiRemote.imageToByteArray(icon, System.Drawing.Imaging.ImageFormat.Png);
                                }
                            }
                        }

                        plugins.Add(new WindowPlugin(plugin.PluginName(), plugin.GetWindowId(), iconBytes));
                    }
                    else
                    {
                        plugins.Add(new WindowPlugin(plugin.PluginName(), plugin.GetWindowId(), null));
                    }
                }
            }

            return plugins;
        }

        /// <summary>
        /// Check if assembly is available
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="ver">Assembly version</param>
        /// <returns>true if the assembly is available</returns>
        internal static bool IsAssemblyAvailable(string name, Version ver)
        {
            bool result = false;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                try
                {
                    if (a.GetName().Name == name)
                    {
                        if (ver == null || a.GetName().Version >= ver)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Starts the status update thread
        /// </summary>
        private void StartNowPlayingUpdateThread()
        {
            if (nowPlayingUpdateThread == null)
            {
                nowPlayingUpdateThread = new Thread(new ThreadStart(DoNowPlayingUpdate));
                nowPlayingUpdateThread.Start();
            }
        }

        /// <summary>
        /// Stops the status update thread
        /// </summary>
        private void StopNowPlayingUpdateThread()
        {
            nowPlayingUpdateThreadRunning = false;
            nowPlayingUpdateThread = null;
        }

        /// <summary>
        /// Updates the current status of the playing item and sends the basic information
        /// (duration, position, speed) to all clients
        /// </summary>
        private void DoNowPlayingUpdate()
        {
            LogMessage("Start now-playing update thread", LogType.Debug);
            nowPlayingUpdateThreadRunning = true;
            while (nowPlayingUpdateThreadRunning)
            {
                if (g_Player.Playing && nowPlayingUpdateThreadRunning)
                {
                    //LogMessage("Send Nowplaying", LogType.Debug);
                    socketServer.sendNowPlayingUpdateToAllClients();
                }
                Thread.Sleep(UPDATE_INTERVAL);
            }
            LogMessage("Stop now-playing update thread", LogType.Debug);
        }
    }
}
