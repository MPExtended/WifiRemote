using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;
using Deusty.Net;
using MediaPortal.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MediaPortal.GUI.Library;
using WifiRemote.Messages;

namespace WifiRemote
{
    /// <summary>
    /// Async socket server
    /// Handles all client connection and data sent to and from them
    /// </summary>
    class SocketServer
    {
        private UInt16 port;

        private AsyncSocket listenSocket;
        private Communication communication;
        private List<AsyncSocket> connectedSockets;
        private AuthMethod allowedAuth;

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
        internal String UserName {get; set;}

        /// <summary>
        /// Password for client authentification
        /// </summary>
        internal String Password {get; set;}

        /// <summary>
        /// Passcode for client authentification
        /// </summary>
        internal String PassCode {get; set;}

        /// <summary>
        /// Passcode for client authentification
        /// </summary>
        internal AuthMethod AllowedAuth 
        {
            get { 
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

            listenSocket = new AsyncSocket();

            // Tell AsyncSocket to allow multi-threaded delegate methods
            listenSocket.AllowMultithreadedCallbacks = true;

            // Register for client connect event
            listenSocket.DidAccept += new AsyncSocket.SocketDidAccept(listenSocket_DidAccept);

            // Initialize list to hold connected sockets
            connectedSockets = new List<AsyncSocket>();

            String welcome = JsonConvert.SerializeObject(welcomeMessage);
            WifiRemote.LogMessage("Client connected, sending welcome msg: " + welcome, WifiRemote.LogType.Debug);
        }


        /// <summary>
        /// Start listening for incoming connections.
        /// </summary>
        public void Start()
        {
            Exception error;
            if (!listenSocket.Accept(port, out error))
            {
                WifiRemote.LogMessage("Error starting server: " + error.Message, WifiRemote.LogType.Error);
                return;
            }

            WifiRemote.LogMessage("Now accepting connections.", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Stop the server and disconnect all clients.
        /// </summary>
        public void Stop()
        {
            // Stop accepting connections
            listenSocket.Close();

            // Stop any client connections
            lock (connectedSockets)
            {
                foreach (AsyncSocket socket in connectedSockets)
                {
                    socket.CloseAfterReading();
                }
            }

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

            WifiRemote.LogMessage("Sending message object to client: " + message.ToString(), WifiRemote.LogType.Debug);
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

            WifiRemote.LogMessage("Sending string message to client:" + message, WifiRemote.LogType.Debug);

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

            foreach (AsyncSocket socket in connectedSockets)
            {
                SendMessageToClient(message, socket, ignoreAuth);
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
        public void SendImageToClient(AsyncSocket sender, String imagePath)
        {
            MessageImage imageMessage = new MessageImage(imagePath);
            SendMessageToClient(imageMessage, sender);
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
            
            // Send basic information package if no auth is needed
            if (AllowedAuth == AuthMethod.None)
            {
                SendAuthenticationResponse(newSocket, true);
                newSocket.GetRemoteClient().IsAuthenticated = true;              
                sendOverviewInformationToClient(newSocket);
            }
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
                // TODO: error checking
                JObject message = JObject.Parse(msg);
                string type = (string)message["Type"];
                RemoteClient client = sender.GetRemoteClient();

                if (client.IsAuthenticated || AllowedAuth == AuthMethod.None)
                {// The client is already authentificated or we don't need authentification
                    // Send a command
                    if (type == "command")
                    {
                        string command = (string)message["Command"];
                        communication.SendCommand(command);
                    }
                    // Send a key press
                    else if (type == "key")
                    {
                        string key = (string)message["Key"];
                        communication.SendKey(key);
                    }
                    // Send a key down
                    else if (type == "commandstartrepeat")
                    {
                        string key = (string)message["Key"];
                        int pause = (int)message["Pause"];
                        communication.SendCommandRepeatStart(key, pause);
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
                    // Shutdown/hibernate/reboot system or exit mediaportal
                    else if (type == "powermode")
                    {
                        string powerMode = (string)message["PowerMode"];
                        communication.SetPowerMode(powerMode);
                    }
                    // Directly set the volume to Volume percent
                    else if (type == "volume")
                    {
                        int volume = (int)message["Volume"];
                        communication.SetVolume(volume);
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
                            double position = (double)message["Position"];
                            communication.SetPosition(position, true);
                        }
                        else if (seekType == 3)
                        {
                            double position = (double)message["Position"];
                            communication.SetPosition(position, false);
                        }
                    }
                    // Start to play a file identified by Filepath
                    else if (type == "playfile")
                    {
                        string fileType = (string)message["FileType"];
                        string filePath = (string)message["Filepath"];

                        // Play a video file
                        if (fileType == "video")
                        {
                            communication.PlayVideoFile(filePath);
                        }
                        // Play an audio file
                        else if (fileType == "audio")
                        {
                            communication.PlayAudioFile(filePath);
                        }
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
                        foreach (JValue v in array)
                        {
                            String propString = (string)v.Value;
                            client.Properties.Add(propString);
                        }
                        SendPropertiesToClient(sender);
                    }
                    // request image
                    else if (type == "image")
                    {
                        String path = (string)message["ImagePath"];
                        SendImageToClient(sender, path);
                    }
                    else
                    {
                        // Unknown command. Log or inform user ...
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
                            SendAuthenticationResponse(sender, true);
                            sendOverviewInformationToClient(sender);
                        }
                        else
                        {
                            // Client sends a message other then authenticate when not yet
                            // authenticated or authenticate failed
                            SendAuthenticationResponse(sender, false);
                        }
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

        private void SendAuthenticationResponse(AsyncSocket socket, bool _success)
        {
            MessageAuthenticationResponse authResponse = new MessageAuthenticationResponse(_success);
            if (!_success)
            {
                authResponse.ErrorMessage = "Login failed";
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
    }
}
