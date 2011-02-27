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

        private MessageWelcome welcomeMessage;
        private MessageStatus statusMessage;
        private MessageVolume volumeMessage;
        private MessageImage imageMessage;
        private MessageNowPlaying nowPlayingMessage;

        // Delegate to log messages from another thread
        private delegate void LogMessage(string message, WifiRemote.LogType type);

        /// <summary>
        /// Constructor.
        /// Initialise and setup the socket server.
        /// </summary>
        public SocketServer(UInt16 port)
        {
            this.communication = new Communication();
            this.welcomeMessage = new MessageWelcome();
            this.statusMessage = new MessageStatus();
            this.volumeMessage = new MessageVolume();
            this.imageMessage = new MessageImage();
            this.nowPlayingMessage = new MessageNowPlaying();

            this.welcomeMessage.Status = this.statusMessage;
            this.welcomeMessage.Volume = this.volumeMessage;

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
        /// Send a message to all connected clients.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToAllClients(String message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\r\n");

            foreach (AsyncSocket socket in connectedSockets)
            {
                socket.Write(data, -1, 0);
            }
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
        public void SendImageToAllClients()
        {
            String image = JsonConvert.SerializeObject(imageMessage);
            SendMessageToAllClients(image);
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
        /// A client connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="newSocket"></param>
        void listenSocket_DidAccept(AsyncSocket sender, AsyncSocket newSocket)
        {
            // Subsribe to worker socket events
            newSocket.DidRead   += new AsyncSocket.SocketDidRead(newSocket_DidRead);
            newSocket.DidWrite  += new AsyncSocket.SocketDidWrite(newSocket_DidWrite);
            newSocket.WillClose += new AsyncSocket.SocketWillClose(newSocket_WillClose);
            newSocket.DidClose  += new AsyncSocket.SocketDidClose(newSocket_DidClose);
            
            // Store worker socket in client list
            lock (connectedSockets)
            {
                connectedSockets.Add(newSocket);
            }

            // Send welcome message to client
            String welcome = JsonConvert.SerializeObject(welcomeMessage);
            WifiRemote.LogMessage("Client connected, sending welcome msg: "+ welcomeMessage.ToString(), WifiRemote.LogType.Debug);

            byte[] data = Encoding.UTF8.GetBytes(welcome + "\r\n");
            newSocket.Write(data, -1, 0);

            // If we are playing a file send detailed information about it
            if (g_Player.Playing)
            {
                String nowPlaying = JsonConvert.SerializeObject(nowPlayingMessage);
                byte[] nowPlayingData = Encoding.UTF8.GetBytes(nowPlaying + "\r\n");
                newSocket.Write(nowPlayingData, -1, 0);
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
                else if (type == "keydown")
                {
                    string key = (string)message["Key"];
                    int pause = (int)message["Pause"];
                    communication.SendKeyDown(key, pause);
                }
                // Send a key up
                else if (type == "keyup")
                {
                    communication.SendKeyUp();
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
                else if (type == "volume")
                {
                    int volume = (int)message["Volume"];
                    communication.SetVolume(volume);
                }
                else if (type == "video")
                {
                    String video = (string)message["Filepath"];
                    communication.PlayVideoFile(video);
                }
                else
                {
                    // Unknown command. Log or inform user ...
                }


                //WifiRemote.LogMessage("Received: " + msg, WifiRemote.LogType.Info);
            }
            catch (Exception e)
            {
                //WifiRemote.LogMessage("Error converting received data into UTF-8 String: " + e.Message, WifiRemote.LogType.Error);
                MediaPortal.Dialogs.GUIDialogNotify dialog = (MediaPortal.Dialogs.GUIDialogNotify)MediaPortal.GUI.Library.GUIWindowManager.GetWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                dialog.Reset();
                dialog.SetHeading("WifiRemote Communication Error");
                dialog.SetText(e.Message);
                dialog.DoModal(MediaPortal.GUI.Library.GUIWindowManager.ActiveWindow);
            }

   
            // Continue listening
            sender.Read(AsyncSocket.CRLFData, -1, 0);
        }
    }
}
