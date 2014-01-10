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
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.ServiceProcess;
using Microsoft.Win32;
using MediaPortal.Dialogs;
using WifiRemote.MPDialogs;
using WifiRemote.Messages;

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
    [PluginIcons("WifiRemote.Resources.logo_radio.png", "WifiRemote.Resources.logo_radio_disabled.png")]
    public class WifiRemote : ISetupForm, IPlugin
    {
        public const string PLUGIN_NAME = "WifiRemote";
        public const string LOG_PREFIX = "[WIFI_REMOTE] ";
        public const int DEFAULT_PORT = 8017;
        public const int SERVER_VERSION = 1;
        private const int UPDATE_INTERVAL = 1000;

        private const string MP_EXTENDED_SERVICE = "MPExtended Service";

        /// <summary>
        /// The localised name of the virtual keyboard
        /// Used to detect if the keyboard was opened or closed
        /// </summary>
        private String localizedKeyboard;

        /// <summary>
        /// Determines if the onscreen keyboard is active
        /// </summary>
        private bool keyboardIsActive;

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

        /// <summary>
        /// <code>true</code> if TVPlugin is loaded
        /// </summary>
        public static bool IsAvailableTVPlugin
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if moving pictures is available
        /// </summary>
        public static bool IsAvailableMovingPictures
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if TV-Series is available
        /// </summary>
        public static bool IsAvailableTVSeries
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if trakt plugin is available
        /// </summary>
        public static bool IsAvailableTrakt
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if Fanart Handler is available
        /// </summary>
        public static bool IsAvailableFanartHandler
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if the MP-Extended service is installed, running
        /// and supporting the MediaAccessService
        /// </summary>
        public static bool IsAvailableMPExtendedMAS
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if the MP-Extended service is installed, running
        /// and supporting the TvAccessService
        /// </summary>
        public static bool IsAvailableMPExtendedTAS
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if the MP-Extended service is installed, running
        /// and supporting the WebStreamingService
        /// </summary>
        public static bool IsAvailableMPExtendedWSS
        {
            get;
            set;
        }

        /// <summary>
        /// <code>true</code> if the MPNotificationBar plugin is installed
        /// </summary>
        public static bool IsAvailableNotificationBar
        {
            get;
            set;
        }

        /// <summary>
        /// Latest channelId
        /// </summary>
        public static int LatestChannelId
        {
            get;
            set;
        }

        /// <summary>
        /// List of plugins
        /// </summary>
        public static ArrayList savedAndSortedPlugins;

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
            return "Shukuyen, DieBagger";
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
            // Check if TV plugin is installed. We need to explicitely check for ArgusTV and ForTheRecord here, as those overwrite the
            // TV Plugin with a placeholder assembly that doesn't have any functions.
            WifiRemote.IsAvailableTVPlugin = IsAssemblyAvailable("TVPlugin", new Version(1, 0, 0, 0)) &&
                !IsAssemblyAvailable("ArgusTV.UI.MediaPortal", null) &&
                !IsAssemblyAvailable("ForTheRecord.UI.MediaPortal", null);
            WifiRemote.IsAvailableMovingPictures = IsAssemblyAvailable("MovingPictures", new Version(1, 0, 6, 1116));
            WifiRemote.IsAvailableTVSeries = IsAssemblyAvailable("MP-TVSeries", new Version(2, 6, 3, 1242));
            WifiRemote.IsAvailableTrakt = IsAssemblyAvailable("TraktPlugin", new Version(3, 0));
            WifiRemote.IsAvailableFanartHandler = IsAssemblyAvailable("FanartHandler", new Version(2, 2, 1, 19191));
            WifiRemote.IsAvailableNotificationBar = IsAssemblyAvailable("MPNotificationBar", new Version(0, 8, 2, 1));

            // Check for MP-Extended
            if (isMPExtendedRunning())
            {
                WifiRemote.IsAvailableMPExtendedMAS = isMPExtendedServiceInstalled("MediaAccessServiceInstalled");
                WifiRemote.IsAvailableMPExtendedTAS = isMPExtendedServiceInstalled("TVAccessServiceInstalled");
                WifiRemote.IsAvailableMPExtendedWSS = isMPExtendedServiceInstalled("StreamingServiceInstalled");
            }
            else
            {
                // Service not started, no MP-Extended functionality
                WifiRemote.IsAvailableMPExtendedMAS = false;
                WifiRemote.IsAvailableMPExtendedTAS = false;
                WifiRemote.IsAvailableMPExtendedWSS = false;
            }

            LatestChannelId = -1;
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

            // Only subscribe to the tv channel changed callback if the tv plugin is installed.
            // Argus users will experience crashes otherwise.
            if (WifiRemote.IsAvailableTVPlugin)
            {
                g_Player.TVChannelChanged += new g_Player.TVChannelChangeHandler(g_Player_TVPlayBackChanged);
            }


            GUIWindowManager.Receivers += new SendMessageHandler(GUIWindowManager_Receivers);

            System.Net.NetworkInformation.NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(SystemEvents_PowerModeChanged);

            // Init and start the socket
            InitAndStartSocket();

            // Publish the service via bonjour to the network
            if (!disableBonjour)
            {
                PublishBonjourService();
            }

            localizedKeyboard = GUILocalizeStrings.Get(100000 + (int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
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
                InitAndStartSocket();

                // Restart bonjour service
                if (!disableBonjour)
                {
                    WifiRemote.LogMessage("Restarting bonjour service", LogType.Debug);
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
                if (socketServer != null)
                {
                    socketServer.Stop();
                }
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
                InitAndStartSocket();

                // Restart bonjour service
                if (!disableBonjour)
                {
                    WifiRemote.LogMessage("Restarting bonjour service", LogType.Debug);
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
                if (socketServer != null)
                {
                    socketServer.Stop();
                }
            }
        }

        /// <summary>
        /// Event handler when a GUI property was changed
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="tagValue"></param>
        void GUIPropertyManager_OnPropertyChanged(string tag, string tagValue)
        {
            if (tag.Equals("#currentmodule"))
            {
                if (tagValue != null && tagValue.Equals(localizedKeyboard))
                {
                    MessageOnscreenKeyboard keyboardMessage = new MessageOnscreenKeyboard(true);
                    string keyboard = Newtonsoft.Json.JsonConvert.SerializeObject(keyboardMessage);
                    socketServer.SendMessageToAllClients(keyboard);
                    keyboardIsActive = true;
                }
                else if (keyboardIsActive)
                {
                    MessageOnscreenKeyboard keyboardMessage = new MessageOnscreenKeyboard(false);
                    string keyboard = Newtonsoft.Json.JsonConvert.SerializeObject(keyboardMessage);
                    socketServer.SendMessageToAllClients(keyboard);
                    keyboardIsActive = false;
                }

                IRenderLayer layer = GUILayerManager.GetLayer(GUILayerManager.LayerType.Dialog);
                if (layer != null && layer.GetType().IsSubclassOf(typeof(GUIDialogWindow)))
                {
                    WifiRemote.LogMessage("Sending dialog open to clients", LogType.Debug);
                    MpDialogsHelper.CurrentDialog = layer as GUIDialogWindow;

                    MessageDialog msg = MpDialogsHelper.GetDialogMessage(MpDialogsHelper.CurrentDialog);

                    if (msg != null && msg.Dialog != null && msg.Dialog.GetType() == typeof(MpDialogMenu))
                    {
                        //TODO: this is a hack to retrieve the list items of a dialog because they are
                        //set after initialisation of the dialog and there is no event fired up after
                        //that. We might run into situations where the list hasn't been updated when
                        //we try to read it
                        Thread t = new Thread(new ParameterizedThreadStart(SendDelayed));
                        t.Start(msg);
                    }
                    else if (msg != null && msg.Dialog != null && msg.Dialog.GetType() == typeof(MpDialogProgress))
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(SendProgressUpdates));
                        t.Start(msg);
                    }
                    else
                    {
                        socketServer.SendMessageToAllClients(msg);
                    }
                    MpDialogsHelper.IsDialogShown = true;
                }
                else if (MpDialogsHelper.IsDialogShown)
                {
                    WifiRemote.LogMessage("Sending dialog close to clients", LogType.Debug);
                    MessageDialog message = new MessageDialog();
                    message.DialogShown = false;
                    socketServer.SendMessageToAllClients(message);
                    MpDialogsHelper.IsDialogShown = false;
                }
            }

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

            if (tag.Equals("#selectedindex") || tag.Equals("#highlightedbutton"))
            {
                socketServer.SendListViewStatusToAllClientsIfChanged();
            }
        }

        /// <summary>
        /// Sends the dialog to all connected socket with a delay so we can
        /// read the list items
        /// </summary>
        /// <param name="_control"></param>
        private void SendDelayed(object _control)
        {
            MessageDialog msg = (MessageDialog)_control;

            WifiRemote.LogMessage("Sending delayed list dialog", LogType.Debug);
            MpDialogMenu dialog = msg.Dialog as MpDialogMenu;

            //get the items from the dialog
            dialog.RetrieveListItems();

            socketServer.SendMessageToAllClients(msg);
        }

        /// <summary>
        /// Sends the dialog to all connected socket with a delay so we can
        /// read the list items
        /// </summary>
        /// <param name="_control"></param>
        private void SendProgressUpdates(object _control)
        {
            MessageDialog msg = (MessageDialog)_control;
            socketServer.SendMessageToAllClients(msg);

            MpDialogProgress dialog = msg.Dialog as MpDialogProgress;
            Thread.Sleep(200);

            //from now on the messages are updating the initial dialog
            msg.DialogUpdate = true;
            while (MpDialogsHelper.IsDialogShown)
            {
                if (dialog.UpdateValues())
                {
                    //dialog values have been changed -> send update
                    socketServer.SendMessageToAllClients(msg);
                }
                Thread.Sleep(200);
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
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_HIGHLIGHT_ITEM:
                case MediaPortal.GUI.Library.Action.ActionType.ACTION_SELECT_ITEM:
                    socketServer.SendListViewStatusToAllClientsIfChanged();
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
            else if (message.Message == GUIMessage.MessageType.GUI_MSG_ITEM_SELECT)
            {
                socketServer.SendListViewStatusToAllClientsIfChanged();
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
        /// Mediaportal TV playback changed
        /// </summary>
        void g_Player_TVPlayBackChanged()
        {
            TvPlugin.TVHome.Navigator.UpdateCurrentChannel();
            TvDatabase.Channel current = TvPlugin.TVHome.Navigator.Channel;

            if (socketServer != null && (LatestChannelId == -1 || LatestChannelId != current.IdChannel))
            {
                LatestChannelId = current.IdChannel;
                LogMessage("TV Playback changed!", LogType.Debug);
                socketServer.SendNowPlayingToAllClients();
            }            
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
            if (socketServer != null)
            {
                socketServer.SendStatusToAllClientsIfChanged();
            }
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
                LogMessage(String.Format("Bonjour version {0} found.", bonjourVersion.ToString()), LogType.Info);
            }
            catch
            {
                LogMessage("Bonjour enabled but not installed! Get it at http://support.apple.com/downloads/Bonjour_for_Windows", LogType.Error);
                LogMessage("Disabling Bonjour for this session.", LogType.Info);
                disableBonjour = true;
                return;
            }

            publishService = new NetService(domain, serviceType, serviceName, port);

            // Get the MAC addresses and set it as bonjour txt record
            // Needed by the clients to implement wake on lan
            Hashtable dict = new Hashtable();
            dict.Add("hwAddr", GetHardwareAddresses());
            publishService.TXTRecordData = NetService.DataFromTXTRecordDictionary(dict);
            publishService.DidPublishService += new NetService.ServicePublished(publishService_DidPublishService);
            publishService.DidNotPublishService += new NetService.ServiceNotPublished(publishService_DidNotPublishService);

            publishService.Publish();
        }

        /// <summary>
        /// Get the hardware (MAC) addresses of all available network adapters
        /// </summary>
        /// <returns>Seperated MAC addresses</returns>
        public static String GetHardwareAddresses()
        {
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

            return hardwareAddresses.ToString();
        }

        /// <summary>
        /// Service couldn't be published
        /// </summary>
        /// <param name="service"></param>
        /// <param name="exception"></param>
        void publishService_DidNotPublishService(NetService service, DNSServiceException exception)
        {
            LogMessage(String.Format("Bonjour publish error: {0}", exception.Message), LogType.Error);
        }

        /// <summary>
        /// Service was published
        /// </summary>
        /// <param name="service"></param>
        void publishService_DidPublishService(NetService service)
        {
            LogMessage("Published Service via Bonjour!", LogType.Info);
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
        /// <returns>The name of the service</returns>
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

        #endregion

        #region WifiRemote methods

        /// <summary>
        /// Initialise the socket server if necessary
        /// </summary>
        internal void InitAndStartSocket()
        {
            if (socketServer == null)
            {
                WifiRemote.LogMessage("Setting up socket server", LogType.Debug);

                String userName = null;
                String password = null;
                String passcode = null;
                AuthMethod auth = AuthMethod.None;
                int autologinTimeout = 0;
                bool showNotification = false;

                // Load port from config
                using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                {
                    port = (UInt16)reader.GetValueAsInt(PLUGIN_NAME, "port", DEFAULT_PORT);
                    disableBonjour = reader.GetValueAsBool(PLUGIN_NAME, "disableBonjour", false);
                    serviceName = reader.GetValueAsString(PLUGIN_NAME, "serviceName", "");
                    userName = reader.GetValueAsString(PLUGIN_NAME, "username", "");
                    userName = WifiRemote.DecryptString(userName);
                    password = reader.GetValueAsString(PLUGIN_NAME, "password", "");
                    password = WifiRemote.DecryptString(password);
                    passcode = reader.GetValueAsString(PLUGIN_NAME, "passcode", "");
                    passcode = WifiRemote.DecryptString(passcode);

                    auth = (AuthMethod)reader.GetValueAsInt(PLUGIN_NAME, "auth", 0);
                    autologinTimeout = reader.GetValueAsInt(PLUGIN_NAME, "autologinTimeout", 0);

                    showNotification = reader.GetValueAsBool(PLUGIN_NAME, "showNotifications", false);

                }

                // Start listening for client connections
                socketServer = new SocketServer(port);
                socketServer.UserName = userName;
                socketServer.Password = password;
                socketServer.PassCode = passcode;
                socketServer.AllowedAuth = auth;
                socketServer.AutologinTimeout = autologinTimeout;
                socketServer.ShowNotifications = showNotification;
            }

            socketServer.Start();
        }


        /// <summary>
        /// Get all active window plugins and the corresponding window IDs.
        /// This can be used in the client to jump to a specific plugin.
        /// 
        /// We are also sending the plugin icon as byte array if it exists.
        /// </summary>
        internal static ArrayList GetActiveWindowPluginsAndIDs(bool sendIcons)
        {
            // Return cached data
            if (WifiRemote.savedAndSortedPlugins != null)
            {
                return WifiRemote.savedAndSortedPlugins;
            }

            // Init cache
            savedAndSortedPlugins = new ArrayList();

            // No cache yet, build plugin list
            ArrayList plugins = new ArrayList();
            ArrayList sortedPlugins = new ArrayList();

            Dictionary<int, String> savedPlugins;
            List<int> ignoredPluginsList;

            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                // Read plugin ids and convert them to int
                String[] savedPluginStrings = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "savedPlugins", "").Split('|');
                savedPlugins = new Dictionary<int, string>();

                for (int j = 0; j + 1 < savedPluginStrings.Length; j = j + 2)
                {
                    // Add plugin id and name
                    int i;
                    if (int.TryParse(savedPluginStrings[j], out i))
                    {
                        savedPlugins.Add(i, savedPluginStrings[j + 1]);
                    }
                }

                // Read ignored plugins
                // Ignored by default: 
                //     -1: 
                //      0: home
                //   3005: GUITopbar
                // 730716: fanart handler
                String[] ignoredPluginsString = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "ignoredPlugins", "-1|0|3005|730716").Split('|');
                ignoredPluginsList = new List<int>();

                foreach (String pluginId in ignoredPluginsString)
                {
                    int i;
                    if (int.TryParse(pluginId, out i))
                    {
                        ignoredPluginsList.Add(i);
                    }
                }
            }

            // Fetch all active plugins
            foreach (ISetupForm plugin in PluginManager.SetupForms)
            {
                // Plugin not hidden
                if (!ignoredPluginsList.Contains(plugin.GetWindowId()))
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
                                    iconBytes = ImageHelper.imageToByteArray(icon, System.Drawing.Imaging.ImageFormat.Png);
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

            // Add sorted plugins
            foreach (var aSavedPlugin in savedPlugins)
            {
                // Find saved plugin with this window id
                var query = from WindowPlugin p in plugins
                            where p.WindowId == aSavedPlugin.Key
                            select p;

                // Add the first found plugin to the list
                foreach (WindowPlugin plugin in query)
                {
                    WifiRemote.savedAndSortedPlugins.Add(new WindowPlugin(aSavedPlugin.Value, aSavedPlugin.Key, plugin.Icon));
                    break;
                }
            }

            // Add rest of plugins
            foreach (WindowPlugin plugin in plugins)
            {
                if (!savedPlugins.ContainsKey(plugin.WindowId))
                {
                    WifiRemote.savedAndSortedPlugins.Add(plugin);
                }
            }

            return WifiRemote.savedAndSortedPlugins;
        }

        /// <summary>
        /// Check if assembly is available
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="ver">Assembly version</param>
        /// <returns>true if the assembly is available</returns>
        internal static bool IsAssemblyAvailable(string name, Version ver)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                try
                {
                    if (a.GetName().Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return (ver == null || a.GetName().Version >= ver);
                    }
                }
                catch { }
            }

            return false;
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

        /// <summary>
        /// Decrypt an encrypted setting string
        /// </summary>
        /// <param name="encrypted">The string to decrypt</param>
        /// <returns>The decrypted string or an empty string if something went wrong</returns>
        internal static string DecryptString(string encrypted)
        {
            string decrypted = String.Empty;

            EncryptDecrypt Crypto = new EncryptDecrypt();
            try
            {
                decrypted = Crypto.Decrypt(encrypted);
            }
            catch (Exception)
            {
                WifiRemote.LogMessage("Could not decrypt config string!", LogType.Error);
                decrypted = null;
            }

            return decrypted;
        }

        /// <summary>
        /// Encrypt a setting string
        /// </summary>
        /// <param name="decrypted">An unencrypted string</param>
        /// <returns>The string encrypted</returns>
        internal static string EncryptString(string decrypted)
        {
            EncryptDecrypt Crypto = new EncryptDecrypt();
            string encrypted = String.Empty;

            try
            {
                encrypted = Crypto.Encrypt(decrypted);
            }
            catch (Exception)
            {
                WifiRemote.LogMessage("Could not encrypt setting string!", LogType.Error);
                encrypted = null;
            }

            return encrypted;
        }

        /// <summary>
        /// Returns <code>true</code> if the MP-Extended service is installed and running
        /// </summary>
        private bool isMPExtendedRunning()
        {
            try
            {
                ServiceController service = new ServiceController(MP_EXTENDED_SERVICE);
                return (service.Status == ServiceControllerStatus.Running);
            }
            catch (Exception) { }

            return false;
        }

        /// <summary>
        /// Check if the specified MP-Extended service is installed
        /// </summary>
        /// <param name="service">Name of the service registry key</param>
        /// <returns><code>true</code> if the service is installed</returns>
        private bool isMPExtendedServiceInstalled(string service)
        {
            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"Software\MPExtended");
            if (regkey == null)
            {
                return false;
            }

            object value = regkey.GetValue(service);
            if (value == null)
            {
                return false;
            }

            return value.ToString() == "true";
        }

        #endregion

    }
}
