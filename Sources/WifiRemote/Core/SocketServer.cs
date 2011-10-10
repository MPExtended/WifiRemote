using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Deusty.Net;
using MediaPortal.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MediaPortal.GUI.Library;
using WifiRemote.Messages;
using WifiRemote.MPPlayList;

namespace WifiRemote
{
    /// <summary>
    /// Async socket server
    /// Handles all client connection and data sent to and from them
    /// </summary>
    class SocketServer
    {
        // Send left shift key to disable screensaver interop
        [DllImport("user32")]
        private static extern void keybd_event(byte bVirtualKey, byte bScanCode, int dwFlags, int dwExtraInfo);

        private const byte VK_LSHIFT = 0xA0;
        private const int KEYEVENTF_KEYUP = 0x0002;


        // SocketServer
        private UInt16 port;

        private bool isStarted = false;

        private AsyncSocket listenSocket;
        private Communication communication;
        private List<AsyncSocket> connectedSockets;
        private AuthMethod allowedAuth;
        private List<AutoLoginToken> loginTokens;

        private MessageWelcome welcomeMessage;
        private MessageStatus statusMessage;
        private MessageVolume volumeMessage;
        private MessageNowPlaying nowPlayingMessage;
        private MessageNowPlayingUpdate nowPlayingMessageUpdate;
        private MessagePropertyChanged nowPlayingPropertiesUpdate;

        // Delegate to log messages from another thread
        private delegate void LogMessage(string message, WifiRemote.LogType type);

        /// <summary>
        /// Username  for client authentification
        /// </summary>
        internal String UserName { get; set; }

        /// <summary>
        /// Password for client authentification
        /// </summary>
        internal String Password { get; set; }

        /// <summary>
        /// Passcode for client authentification
        /// </summary>
        internal String PassCode { get; set; }

        /// <summary>
        /// Time in minutes that an authenticated client is
        /// able to send commands without authenticating again.
        /// 
        /// 0 to disable autologin.
        /// </summary>
        internal int AutologinTimeout { get; set; }

        /// <summary>
        /// Passcode for client authentification
        /// </summary>
        internal AuthMethod AllowedAuth
        {
            get
            {
                return allowedAuth;
            }

            set
            {
                allowedAuth = value;
                this.welcomeMessage.AuthMethod = allowedAuth;
            }
        }

        /// <summary>
        /// Constructor.
        /// Initialise and setup the socket server.
        /// </summary>
        public SocketServer(UInt16 port)
        {
            this.communication = new Communication();
            this.welcomeMessage = new MessageWelcome();
            welcomeMessage.AuthMethod = AllowedAuth;
            this.statusMessage = new MessageStatus();
            this.volumeMessage = new MessageVolume();
            this.nowPlayingMessage = new MessageNowPlaying();
            this.nowPlayingMessageUpdate = new MessageNowPlayingUpdate();
            this.nowPlayingPropertiesUpdate = new MessagePropertyChanged();

            this.port = port;

            initSocket();
        }

        /// <summary>
        /// Initialise the socket
        /// </summary>
        private void initSocket()
        {
            listenSocket = new AsyncSocket();

            // Tell AsyncSocket to allow multi-threaded delegate methods
            listenSocket.AllowMultithreadedCallbacks = true;

            // Register for client connect event
            listenSocket.DidAccept += new AsyncSocket.SocketDidAccept(listenSocket_DidAccept);

            // Initialize list to hold connected sockets
            connectedSockets = new List<AsyncSocket>();
        }


        /// <summary>
        /// Start listening for incoming connections.
        /// </summary>
        public void Start()
        {
            // Abort if already started
            if (isStarted)
            {
                WifiRemote.LogMessage("ListenSocket already accepting connections, aborting start ...", WifiRemote.LogType.Debug);
                return;
            }

            if (listenSocket == null)
            {
                initSocket();
            }

            Exception error;
            if (!listenSocket.Accept(port, out error))
            {
                WifiRemote.LogMessage("Error starting server: " + error.Message, WifiRemote.LogType.Error);
                return;
            }

            isStarted = true;
            loginTokens = new List<AutoLoginToken>();
            WifiRemote.LogMessage("Now accepting connections.", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Stop the server and disconnect all clients.
        /// </summary>
        public void Stop()
        {
            if (!isStarted)
            {
                WifiRemote.LogMessage("ListenSocket already stopped, ignoring stop command", WifiRemote.LogType.Debug);
                return;
            }

            // Stop accepting connections
            listenSocket.Close();

            // Stop any client connections
            lock (connectedSockets)
            {
                foreach (AsyncSocket socket in connectedSockets)
                {
                    //socket.CloseAfterReading();
                    socket.Close();
                }
            }

            isStarted = false;
            listenSocket = null;

            WifiRemote.LogMessage("SocketServer stopped.", WifiRemote.LogType.Info);
        }


        /// <summary>
        /// Send a message (object) to a specific client
        /// </summary>
        /// <param name="message">Message object to send</param>
        /// <param name="client">A connected client socket</param>
        /// <param name="ignoreAuth">False if messages should only be sent to authed clients</param>
        public void SendMessageToClient(IMessage message, AsyncSocket client, bool ignoreAuth)
        {
            if (message == null)
            {
                WifiRemote.LogMessage("SendMessageToClient failed: IMessage object is null", WifiRemote.LogType.Debug);
                return;
            }

            string messageString = JsonConvert.SerializeObject(message);
            SendMessageToClient(messageString, client, ignoreAuth);
        }

        /// <summary>
        /// Send a message (object) to a specific authed client
        /// </summary>
        /// <param name="message"></param>
        /// <param name="client"></param>
        public void SendMessageToClient(IMessage message, AsyncSocket client)
        {
            SendMessageToClient(message, client, false);
        }

        /// <summary>
        /// Send a message to a specific client
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="client">A connected client socket</param>
        /// <param name="ignoreAuth">False if messages should only be sent to authed clients</param>
        public void SendMessageToClient(String message, AsyncSocket client, bool ignoreAuth)
        {
            if (message == null)
            {
                WifiRemote.LogMessage("SendMessageToClient failed: Message string is null", WifiRemote.LogType.Debug);
                return;
            }

            byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");
            if (client.GetRemoteClient().IsAuthenticated || ignoreAuth)
            {
                client.Write(data, -1, 0);
            }
        }

        /// <summary>
        /// Send a message to a specific authenticated client
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="client">A connected and authenticated client</param>
        public void SendMessageToClient(String message, AsyncSocket client)
        {
            SendMessageToClient(message, client, false);
        }


        /// <summary>
        /// Send a message (object) to all connected clients.
        /// </summary>
        /// <param name="message">Message object to send</param>
        /// <param name="ignoreAuth">False if the message should only be sent to authed clients</param>
        public void SendMessageToAllClients(IMessage message, bool ignoreAuth)
        {
            if (message == null) return;

            foreach (AsyncSocket socket in connectedSockets)
            {
                SendMessageToClient(message, socket, ignoreAuth);
            }
        }

        /// <summary>
        /// Send a message (object) to all connected clients.
        /// </summary>
        /// <param name="message">Message object to send</param>
        public void SendMessageToAllClients(IMessage message)
        {
            SendMessageToAllClients(message, false);
        }

        /// <summary>
        /// Send a message to all connected clients.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="ignoreAuth">False if the message should only be sent to authed clients</param>
        public void SendMessageToAllClients(String message, bool ignoreAuth)
        {
            if (message == null) return;
            lock (connectedSockets)
            {
                foreach (AsyncSocket socket in connectedSockets)
                {
                    SendMessageToClient(message, socket, ignoreAuth);
                }
            }
        }

        /// <summary>
        /// Send a message to all connected and authenticated clients.
        /// </summary>
        /// <param name="message">The message</param>
        public void SendMessageToAllClients(String message)
        {
            SendMessageToAllClients(message, false);
        }

        /// <summary>
        /// Send status to all clients only if it was changed
        /// </summary>
        public void SendStatusToAllClientsIfChanged()
        {
            if (statusMessage.IsChanged())
            {
                SendStatusToAllClients();
            }
        }

        /// <summary>
        /// Send the current player status to all connected clients
        /// </summary>
        public void SendStatusToAllClients()
        {
            String status = JsonConvert.SerializeObject(statusMessage);
            SendMessageToAllClients(status);
        }

        /// <summary>
        /// Send the current volume to all connected clients
        /// </summary>
        public void SendVolumeToAllClients()
        {
            String volume = JsonConvert.SerializeObject(volumeMessage);
            SendMessageToAllClients(volume);
        }

        /// <summary>
        /// Send the image of the currently played media to all clients as byte array
        /// </summary>
        public void SendImageToClient(AsyncSocket sender, String imagePath, String userTag, int width, int height)
        {
            MessageImage imageMessage = new MessageImage(imagePath, userTag, width, height);
            SendMessageToClient(imageMessage, sender);
        }

        /// <summary>
        /// Send the image of the currently played media to all clients as byte array
        /// </summary>
        public void SendImageToClient(AsyncSocket sender, String imagePath)
        {
            SendImageToClient(sender, imagePath, String.Empty, 0, 0);
        }

        /// <summary>
        /// Send the now playing media info to all clients
        /// </summary>
        public void SendNowPlayingToAllClients()
        {
            String nowPlaying = JsonConvert.SerializeObject(nowPlayingMessage);
            WifiRemote.LogMessage(nowPlaying, WifiRemote.LogType.Info);
            SendMessageToAllClients(nowPlaying);
        }

        /// <summary>
        /// Send the now playing update (only basic information) to all clients
        /// </summary>
        internal void sendNowPlayingUpdateToAllClients()
        {
            if (g_Player.Playing)
            {
                String nowPlaying = JsonConvert.SerializeObject(nowPlayingMessageUpdate);
                SendMessageToAllClients(nowPlaying);
            }
        }

        /// <summary>
        /// Send the now playing properties (information that is shown on the mediaportal overlays) 
        /// to all clients
        /// </summary>
        internal void SendNowPlayingPropertiesToAllClients()
        {
            String nowPlaying = JsonConvert.SerializeObject(nowPlayingPropertiesUpdate);
            SendMessageToAllClients(nowPlaying);
        }

        /// <summary>
        /// A client connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newSocket"></param>
        void listenSocket_DidAccept(AsyncSocket sender, AsyncSocket newSocket)
        {
            // Subsribe to worker socket events
            newSocket.DidRead += new AsyncSocket.SocketDidRead(newSocket_DidRead);
            newSocket.DidWrite += new AsyncSocket.SocketDidWrite(newSocket_DidWrite);
            newSocket.WillClose += new AsyncSocket.SocketWillClose(newSocket_WillClose);
            newSocket.DidClose += new AsyncSocket.SocketDidClose(newSocket_DidClose);

            newSocket.SetRemoteClient(new RemoteClient());

            // Store worker socket in client list
            lock (connectedSockets)
            {
                connectedSockets.Add(newSocket);
            }

            // Send welcome message to client
            WifiRemote.LogMessage("Client connected, sending welcome msg.", WifiRemote.LogType.Debug);
            SendMessageToClient(welcomeMessage, newSocket, true);
        }

        /// <summary>
        /// A client closed the connection.
        /// </summary>
        /// <param name="sender"></param>
        void newSocket_DidClose(AsyncSocket sender)
        {
            // Remove the client from the client list.
            lock (connectedSockets)
            {
                WifiRemote.LogMessage("removing client " + sender.GetRemoteClient().ClientName + " from connected sockets", WifiRemote.LogType.Info);
                connectedSockets.Remove(sender);
            }
        }

        /// <summary>
        /// A client will disconnect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newSocket_WillClose(AsyncSocket sender, Exception e)
        {
            WifiRemote.LogMessage("A client is about to disconnect.", WifiRemote.LogType.Debug);
        }

        /// <summary>
        /// The client sent a message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tag"></param>
        void newSocket_DidWrite(AsyncSocket sender, long tag)
        {
            sender.Read(AsyncSocket.CRLFData, -1, 0);
        }

        /// <summary>
        /// Read a message from the client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        /// <param name="tag"></param>
        void newSocket_DidRead(AsyncSocket sender, byte[] data, long tag)
        {
            String msg = null;

            try
            {
                msg = Encoding.UTF8.GetString(data);

                // Get json object
                JObject message = JObject.Parse(msg);
                string type = (string)message["Type"];
                RemoteClient client = sender.GetRemoteClient();

                // Autologin handling
                //
                // Has to be activated in WifiRemote configuration.
                string clientKey = (string)message["AutologinKey"];

                // Key is set: try to authenticate by AutoLoginKey
                if (clientKey != null && !client.IsAuthenticated)
                {
                    if (AutologinTimeout > 0)
                    {
                        AutoLoginToken token = new AutoLoginToken(clientKey, client);
                        // the client token is in the list
                        foreach (AutoLoginToken aToken in loginTokens)
                        {
                            if (aToken.Key == token.Key)
                            {
                                // Check if the autologin key was issued within the timeout
                                TimeSpan elapsed = DateTime.Now - aToken.Issued;
                                client.IsAuthenticated = (elapsed.Minutes < AutologinTimeout);
                                client = aToken.Client;

                                // Renew the timeout
                                aToken.Issued = DateTime.Now;
                            }
                        }

                        // MediaPortal was rebooted (will wipe all AutoLoginKeys) or
                        // autologin time out period is over (configurable in settings).
                        //
                        // Tell the client to reauthenticate.
                        if (!client.IsAuthenticated)
                        {
                            WifiRemote.LogMessage("AutoLoginToken timed out. Client needs to reauthenticate.", WifiRemote.LogType.Debug);
                            TellClientToReAuthenticate(sender);
                            return;
                        }
                    }
                    else
                    {
                        WifiRemote.LogMessage("AutoLogin is disabled but client tried to auto-authenticate.", WifiRemote.LogType.Debug);
                        TellClientToReAuthenticate(sender);
                        return;
                    }
                }

                // The client is already authentificated or we don't need authentification
                if (type != null && client.IsAuthenticated && type != "identify")
                {
                    // Send a command
                    if (type == "command")
                    {
                        string command = (string)message["Command"];
                        if (command != null)
                        {
                            communication.SendCommand(command);
                        }
                    }
                    // Send a key press
                    else if (type == "key")
                    {
                        string key = (string)message["Key"];
                        if (key != null)
                        {
                            communication.SendKey(key);
                        }
                    }
                    // Send a key down
                    else if (type == "commandstartrepeat")
                    {
                        string command = (string)message["Command"];
                        int pause = (int)message["Pause"];
                        if (command != null)
                        {
                            communication.SendCommandRepeatStart(command, pause);
                        }
                    }
                    // Send a key up
                    else if (type == "commandstoprepeat")
                    {
                        communication.SendCommandRepeatStop();
                    }
                    // Open a skin window
                    else if (type == "window")
                    {
                        int windowId = (int)message["Window"];
                        communication.OpenWindow(windowId);
                    }
                    // Activate a window without resetting last activity
                    else if (type == "activatewindow")
                    {
                        int windowId = (int)message["Window"];
                        string param = (string)message["Parameter"];

                        communication.ActivateWindow(windowId, param);
                    }
                    // Shutdown/hibernate/reboot system or exit mediaportal
                    else if (type == "powermode")
                    {
                        string powerMode = (string)message["PowerMode"];
                        if (powerMode != null)
                        {
                            communication.SetPowerMode(powerMode);
                        }
                    }
                    // Directly set the volume to Volume percent
                    else if (type == "volume")
                    {
                        int volume = (int)message["Volume"];
                        bool relative = false;
                        if (message["Relative"] != null)
                        {
                            relative = (bool)message["Relative"];
                        }

                        communication.SetVolume(volume, relative);
                    }
                    // Set the position of the media item
                    else if (type == "position")
                    {
                        int seekType = (int)message["SeekType"];

                        if (seekType == 0)
                        {
                            int position = (int)message["Position"];
                            communication.SetPositionPercent(position, true);
                        }
                        if (seekType == 1)
                        {
                            int position = (int)message["Position"];
                            communication.SetPositionPercent(position, false);
                        }
                        if (seekType == 2)
                        {
                            int position = (int)message["Position"];
                            communication.SetPosition(position, true);
                        }
                        else if (seekType == 3)
                        {
                            int position = (int)message["Position"];
                            communication.SetPosition(position, false);
                        }
                    }
                    // Start to play a file identified by Filepath
                    else if (type == "playfile")
                    {
                        string fileType = (string)message["FileType"];
                        string filePath = (string)message["Filepath"];
                        int startPos = (message["StartPosition"] != null) ? (int)message["StartPosition"] : 0;

                        if (fileType != null && filePath != null)
                        {
                            // Play a video file
                            if (fileType == "video")
                            {
                                communication.PlayVideoFile(filePath, startPos);
                            }
                            // Play an audio file
                            else if (fileType == "audio")
                            {
                                communication.PlayAudioFile(filePath, startPos);
                            }
                        }
                    }
                    // play a tv channel on the client
                    else if (type == "playchannel")
                    {
                        int channelId = (int)message["ChannelId"];
                        bool startFullscreen = (message["StartFullscreen"] != null) ? (bool)message["StartFullscreen"] : false;
                        WifiRemote.LogMessage("playchannel: channelId: " + channelId + " fullscreen: " + startFullscreen, WifiRemote.LogType.Debug);
                        communication.PlayTvChannel(channelId, startFullscreen);
                    }
                    // Reply with a list of installed and active window plugins
                    // with icon and windowId
                    else if (type == "plugins")
                    {
                        bool sendIcons = false;
                        if (message["SendIcons"] != null)
                        {
                            sendIcons = (bool)message["SendIcons"];
                        }
                        SendWindowPluginsList(sender, sendIcons);
                    }
                    // register for a list of properties
                    else if (type == "properties")
                    {
                        client.Properties = new List<String>();
                        JArray array = (JArray)message["Properties"];
                        if (array != null)
                        {
                            foreach (JValue v in array)
                            {
                                String propString = (string)v.Value;
                                client.Properties.Add(propString);
                            }
                            SendPropertiesToClient(sender);
                        }
                    }
                    // request image
                    else if (type == "image")
                    {
                        String path = (string)message["ImagePath"];
                        if (path != null)
                        {
                            int imageWidth = (message["MaximumWidth"] != null) ? (int)message["MaximumWidth"] : 0;
                            int imageHeight = (message["MaximumHeight"] != null) ? (int)message["MaximumHeight"] : 0;
                            SendImageToClient(sender, path, (string)message["UserTag"], imageWidth, imageHeight);
                        }
                    }
                    //playlist actions
                    else if (type == "playlist")
                    {
                        String action = (string)message["PlaylistAction"];
                        String playlistType = (message["PlaylistType"] != null) ? (string)message["PlaylistType"] : "music";
                        bool shuffle = (message["Shuffle"] != null) ? (bool)message["Shuffle"] : false;
                        bool autoPlay = (message["AutoPlay"] != null) ? (bool)message["AutoPlay"] : false;
                        bool showPlaylist = (message["ShowPlaylist"] != null) ? (bool)message["ShowPlaylist"] : true;

                        if (action.Equals("new") || action.Equals("append"))
                        {
                            
                            int insertIndex = 0;
                            if (message["InsertIndex"] != null)
                            {
                                insertIndex = (int)message["InsertIndex"];
                            }

                            // Add items from JSON or SQL
                            JArray array = (message["PlaylistItems"] != null) ? (JArray)message["PlaylistItems"] : null;
                            JObject sql = (message["PlayListSQL"] != null) ? (JObject)message["PlayListSQL"] : null;
                            if (array != null || sql != null)
                            {
                                if (action.Equals("new"))
                                {
                                    PlaylistHelper.ClearPlaylist(playlistType);
                                }

                                int index = insertIndex;

                                if (array != null)
                                {
                                    // Add items from JSON
                                    foreach (JObject o in array)
                                    {
                                        PlaylistEntry entry = new PlaylistEntry();
                                        entry.FileName = (o["FileName"] != null) ? (string)o["FileName"] : null;
                                        entry.Name = (o["Name"] != null) ? (string)o["Name"] : null;
                                        entry.Duration = (o["Duration"] != null) ? (int)o["Duration"] : 0;
                                        PlaylistHelper.AddSongToPlaylist(playlistType, entry, index);
                                        index++;
                                    }

                                    if (shuffle)
                                    {
                                        PlaylistHelper.Shuffle(playlistType);
                                    }
                                }
                                else
                                {
                                    // Add items with SQL
                                    string where = (sql["Where"] != null) ? (string)sql["Where"] : String.Empty;
                                    int limit = (sql["Limit"] != null) ? (int)sql["Limit"] : 0;

                                    PlaylistHelper.AddSongsToPlaylistWithSQL(playlistType, where, limit, shuffle, insertIndex);
                                }

                                if (autoPlay)
                                {
                                    if (message["StartPosition"] != null)
                                    {
                                        int startPos = (int)message["StartPosition"];
                                        insertIndex += startPos;
                                    }
                                    PlaylistHelper.StartPlayingPlaylist(playlistType, insertIndex, showPlaylist);
                                }
                            }
                        }
                        else if (action.Equals("load"))
                        {
                            string playlistName = (string)message["PlayListName"];
                            
                            if (!string.IsNullOrEmpty(playlistName))
                            {
                                PlaylistHelper.LoadPlaylist(playlistType, playlistName, shuffle);
                                if (autoPlay)
                                {
                                    PlaylistHelper.StartPlayingPlaylist(playlistType, 0, showPlaylist);
                                }
                            }                          
                        }
                        else if (action.Equals("get"))
                        {
                            List<PlaylistEntry> items = PlaylistHelper.GetPlaylistItems(playlistType);

                            MessagePlaylist returnPlaylist = new MessagePlaylist();
                            returnPlaylist.PlaylistType = type;
                            returnPlaylist.PlaylistItems = items;

                            SendMessageToClient(returnPlaylist, sender);
                        }
                        else if (action.Equals("remove"))
                        {
                            int indexToRemove = (message["Index"] != null) ? (int)message["Index"] : 0;

                            PlaylistHelper.RemoveSongFromPlaylist(playlistType, indexToRemove);
                        }
                        else if (action.Equals("move"))
                        {
                            int oldIndex = (message["OldIndex"] != null) ? (int)message["OldIndex"] : 0;
                            int newIndex = (message["NewIndex"] != null) ? (int)message["NewIndex"] : 0;
                            PlaylistHelper.ChangePlaylistItemPosition(playlistType, oldIndex, newIndex);
                        }
                        else if (action.Equals("play"))
                        {
                            int index = (message["Index"] != null) ? (int)message["Index"] : 0;
                            PlaylistHelper.StartPlayingPlaylist(playlistType, index, showPlaylist);
                        }
                        else if (action.Equals("clear"))
                        {
                            PlaylistHelper.ClearPlaylist(playlistType);
                        }
                    }
                    // Send the current status to the client
                    else if (type == "requeststatus")
                    {
                        SendMessageToClient(statusMessage, sender);
                    }
                    // Send the current now playing message to the client
                    else if (type == "requestnowplaying")
                    {
                        SendMessageToClient(nowPlayingMessage, sender);
                    }
                    // MovingPictures related commands
                    else if (type == "movingpictures")
                    {
                        if (WifiRemote.IsAvailableMovingPictures)
                        {
                            string action = (string)message["Action"];

                            if (!string.IsNullOrEmpty(action))
                            {
                                // Show movie details for this movie
                                if (action == "moviedetails")
                                {
                                    string movieName = (string)message["MovieName"];
                                    if (!string.IsNullOrEmpty(movieName))
                                    {
                                        int movieId = MovingPicturesHelper.GetMovieIdByName(movieName);
                                        if (movieId > 0)
                                        {
                                            communication.ActivateWindow(96742, "movieid:" + movieId.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            WifiRemote.LogMessage("MovingPictures not installed but required!", WifiRemote.LogType.Error);
                        }
                    }
                    else
                    {
                        // Unknown command. Log or inform user ...
                        WifiRemote.LogMessage("Unknown command received from client " + client.ToString() + ": " + type, WifiRemote.LogType.Info);
                    }
                }
                else
                {
                    // user is not yet authenticated
                    if (type == "identify")
                    {
                        // Save client name if supplied
                        if (message["Name"] != null)
                        {
                            client.ClientName = (string)message["Name"];
                        }

                        // Save client description if supplied
                        if (message["Description"] != null)
                        {
                            client.ClientDescription = (string)message["Description"];
                        }

                        // Save application name if supplied
                        if (message["Application"] != null)
                        {
                            client.ApplicationName = (string)message["Application"];
                        }

                        // Save application version if supplied
                        if (message["Version"] != null)
                        {
                            client.ApplicationVersion = (string)message["Version"];
                        }

                        // Authentication
                        if (AllowedAuth == AuthMethod.None || (message["Authenticate"] != null &&
                            CheckAuthenticationRequest(client, (JObject)message["Authenticate"])))
                        {
                            // User successfully authenticated
                            sender.GetRemoteClient().IsAuthenticated = true;
                            SendAuthenticationResponse(sender, true);
                            sendOverviewInformationToClient(sender);

                            // Turn on display
                            keybd_event(VK_LSHIFT, 0x45, KEYEVENTF_KEYUP, 0);
                        }
                        else
                        {
                            // Client sends a message other then authenticate when not yet
                            // authenticated or authenticate failed
                            SendAuthenticationResponse(sender, false);
                        }
                    }
                    else
                    {
                        // Client needs to authenticate first
                        TellClientToReAuthenticate(sender);
                    }
                }


                //WifiRemote.LogMessage("Received: " + msg, WifiRemote.LogType.Info);
            }
            catch (Exception e)
            {
                WifiRemote.LogMessage("WifiRemote Communication Error: " + e.Message, WifiRemote.LogType.Warn);
                //WifiRemote.LogMessage("Error converting received data into UTF-8 String: " + e.Message, WifiRemote.LogType.Error);
                //MediaPortal.Dialogs.GUIDialogNotify dialog = (MediaPortal.Dialogs.GUIDialogNotify)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                //dialog.Reset();
                //dialog.SetHeading("WifiRemote Communication Error");
                //dialog.SetText(e.Message);
                //dialog.DoModal(MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow);
            }

            // Continue listening
            sender.Read(AsyncSocket.CRLFData, -1, 0);
        }

        private bool CheckAuthenticationRequest(RemoteClient client, JObject message)
        {
            AuthMethod auth = AllowedAuth;

            // For AuthMethod.Both we have to check which method was choosen.
            if (AllowedAuth == AuthMethod.Both)
            {
                if (message["AuthMethod"] == null)
                {
                    WifiRemote.LogMessage("User " + client.ToString() + " authentification failed, no authMethod submitted", WifiRemote.LogType.Info);
                    return false;
                }
                else
                {
                    String authString = (string)message["AuthMethod"];
                    if (authString != null)
                    {
                        if (authString.Equals("userpass"))
                        {
                            auth = AuthMethod.UserPassword;
                        }
                        else if (authString.Equals("passcode"))
                        {
                            auth = AuthMethod.Passcode;
                        }
                        else
                        {
                            WifiRemote.LogMessage("User " + client.ToString() + " authentification failed, invalid authMethod '" + authString + "'", WifiRemote.LogType.Info);
                            return false;
                        }
                    }
                }
            }

            // Check user credentials
            if (auth == AuthMethod.UserPassword)
            {
                if (message["User"] != null && message["Password"] != null)
                {
                    String user = (string)message["User"];
                    String pass = (string)message["Password"];
                    if (user.Equals(this.UserName) && pass.Equals(this.Password))
                    {
                        client.AuthenticatedBy = auth;
                        client.User = user;
                        client.Password = pass;
                        client.IsAuthenticated = true;
                        WifiRemote.LogMessage("User " + client.ToString() + " successfully authentificated by username and password", WifiRemote.LogType.Debug);
                        return true;
                    }
                }
            }
            else if (auth == AuthMethod.Passcode)
            {
                if (message["PassCode"] != null)
                {
                    String pass = (string)message["PassCode"];
                    if (pass.Equals(this.PassCode))
                    {
                        client.AuthenticatedBy = auth;
                        client.PassCode = pass;
                        client.IsAuthenticated = true;
                        WifiRemote.LogMessage("User " + client.ToString() + " successfully authentificated by passcode", WifiRemote.LogType.Debug);
                        return true;
                    }
                }
            }
            else if (auth == AuthMethod.None)
            {
                // Every auth request is valid for AuthMethod.None
                return true;
            }

            WifiRemote.LogMessage("User " + client.ToString() + " authentification failed", WifiRemote.LogType.Info);
            return false;
        }

        /// <summary>
        /// Send a "You need to authenticate yourself." error followed by the
        /// welcome message.
        /// </summary>
        /// <param name="socket"></param>
        private void TellClientToReAuthenticate(AsyncSocket socket)
        {
            MessageAuthenticationResponse response = new MessageAuthenticationResponse(false);
            response.ErrorMessage = "You need to authenticate yourself.";
            SendMessageToClient(response, socket, true);
            SendMessageToClient(welcomeMessage, socket, true);
        }

        private void SendAuthenticationResponse(AsyncSocket socket, bool _success)
        {
            MessageAuthenticationResponse authResponse = new MessageAuthenticationResponse(_success);
            if (!_success)
            {
                authResponse.ErrorMessage = "Login failed";
            }
            else
            {
                WifiRemote.LogMessage("Client identified: " + socket.GetRemoteClient().ToString(), WifiRemote.LogType.Debug);
                string key = getRandomMD5();
                authResponse.AutologinKey = key;
                loginTokens.Add(new AutoLoginToken(key, socket.GetRemoteClient()));
            }

            SendMessageToClient(authResponse, socket, true);
        }


        /// <summary>
        /// Sends a list of installed and active window plugins to the client.
        /// This contains plugin name, icon and windowID.
        /// </summary>
        /// <param name="client">A connected socket client</param>
        /// <param name="sendIcons">Send icons with th eplugin list</param>
        public void SendWindowPluginsList(AsyncSocket client, bool sendIcons)
        {
            MessagePlugins pluginsMessage = new MessagePlugins(sendIcons);
            SendMessageToClient(pluginsMessage, client);
        }

        /// <summary>
        /// Sends all properties a client has registered for to the client
        /// </summary>
        /// <param name="socket">Which client</param>
        private void SendPropertiesToClient(AsyncSocket socket)
        {
            RemoteClient client = socket.GetRemoteClient();
            MessageProperties propertiesMessage = new MessageProperties();

            List<Property> properties = new List<Property>();
            foreach (String s in client.Properties)
            {
                String value = GUIPropertyManager.GetProperty(s);

                if (value != null && !value.Equals("") && CheckProperty(s))
                {
                    properties.Add(new Property(s, value));
                }
            }

            propertiesMessage.Tags = properties;
            SendMessageToClient(propertiesMessage, socket);
        }

        /// <summary>
        /// Checks if the given property should be returned. In some situation we don't
        /// want to return the property, even though the client has registered it.
        /// 
        /// For example, if tv is playing, some video-related tags are filled with faulty
        /// information
        /// </summary>
        /// <param name="tag">The tag</param>
        /// <returns>True if the property should be returned, false otherwise</returns>
        private bool CheckProperty(String tag)
        {
            //don't send these values when tv is playing because they're filled
            //with wrong information (mp problem)
            if (g_Player.Playing && g_Player.IsTV && tag.Equals("#Play.Current.Title")
                || tag.Equals("#Play.Current.Description")
                || tag.Equals("#Play.Current.Genre"))
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// Sends the property to all clients who have registered for it
        /// </summary>
        /// <param name="tag">name of the property</param>
        /// <param name="tagValue">value of the property</param>
        public void SendPropertyToClient(string tag, string tagValue)
        {
            try
            {
                if (!CheckProperty(tag))
                {
                    return;
                }

                byte[] messageData = null;

                if (connectedSockets != null)
                {
                    lock (connectedSockets)
                    {
                        foreach (AsyncSocket socket in connectedSockets)
                        {
                            RemoteClient client = socket.GetRemoteClient();
                            if (client.IsAuthenticated && client.Properties != null)
                            {
                                MessagePropertyChanged changed = null;
                                foreach (String t in client.Properties)
                                {
                                    if (t.Equals(tag))
                                    {
                                        if (messageData == null)
                                        {

                                            //init variable only when at least on client has it on the request list
                                            changed = new MessagePropertyChanged(tag, tagValue);
                                            WifiRemote.LogMessage("Changed property: " + tag + "|" + tagValue, WifiRemote.LogType.Debug);
                                        }
                                        SendMessageToClient(changed, socket);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WifiRemote.LogMessage(ex.Message, WifiRemote.LogType.Error);
            }
        }

        /// <summary>
        /// Send current status, volume and nowplaying info to a client
        /// </summary>
        /// <param name="client"></param>
        private void sendOverviewInformationToClient(AsyncSocket client)
        {
            SendMessageToClient(this.statusMessage, client);
            SendMessageToClient(this.volumeMessage, client);

            // If we are playing a file send detailed information about it
            if (g_Player.Playing)
            {
                SendMessageToClient(this.nowPlayingMessage, client);
            }
        }

        /// <summary>
        /// Get a random md5 hash
        /// </summary>
        /// <returns></returns>
        private String getRandomMD5()
        {
            string randomString = System.IO.Path.GetRandomFileName();
            randomString = randomString.Replace(".", "");

            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] randomBytes = System.Text.Encoding.UTF8.GetBytes(randomString);
            randomBytes = md5.ComputeHash(randomBytes);
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            foreach (byte b in randomBytes)
            {
                hash.Append(b.ToString("x2").ToLower());
            }

            return hash.ToString();
        }
    }
}
