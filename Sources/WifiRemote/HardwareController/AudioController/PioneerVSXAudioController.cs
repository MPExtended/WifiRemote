using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deusty.Net;

namespace WifiRemote.HardwareController.AudioController
{
    class PioneerVSXAudioController : IAudioController
    {
        private AsyncSocket socket;
        private List<String> commandQueue;
        private String ip;

        public PioneerVSXAudioController(String _ip)
        {
            ip = _ip;
            commandQueue = new List<string>();

            socket = new AsyncSocket();
            socket.DidRead += new AsyncSocket.SocketDidRead(socket_DidRead);
            socket.DidWrite += new AsyncSocket.SocketDidWrite(socket_DidWrite);
            socket.DidConnect += new AsyncSocket.SocketDidConnect(socket_DidConnect);
            socket.DidClose += new AsyncSocket.SocketDidClose(socket_DidClose);
        }

        private void sendCommand(String command) 
        {
            // Prepare command for sending
            command = command + "\r\n";
            
            // Check if connected to receiver
            if (!socket.SmartConnected)
            {
                // Add command to queue for later execution
                commandQueue.Add(command);

                if (!socket.Connect(ip, 8102))
                {
                    WifiRemote.LogMessage("Failed to connect to Pioneer receiver", WifiRemote.LogType.Error);
                }
            }
            else
            {
                byte[] data = Encoding.ASCII.GetBytes(command);
                socket.Write(data, -1, 0);
            }
        }


        #region IAudioController Methods
        public void VolumeUp()
        {
            sendCommand("VU");
        }

        public void VolumeDown()
        {
            sendCommand("VD");
        }

        public void ToggleMute()
        {
            sendCommand("MZ");
        }

        /// <summary>
        /// Sets the volume of the client
        /// Not possible with Pioneer over LAN.
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        /// <param name="relative">True if the volume should be changed relative to the current volume</param>
        public void SetVolume(int volume, bool relative)
        {
            
        }
        #endregion



        #region Socket events
        void socket_DidClose(AsyncSocket sender)
        {
            WifiRemote.LogMessage("Socket connection to Pioneer receiver closed", WifiRemote.LogType.Debug);
        }

        void socket_DidConnect(AsyncSocket sender, System.Net.IPAddress address, ushort port)
        {
            foreach (String command in commandQueue)
            {
                byte[] data = Encoding.ASCII.GetBytes(command);
                socket.Write(data, -1, 0);
            }

            commandQueue.Clear();
        }

        void socket_DidWrite(AsyncSocket sender, long tag)
        {
            WifiRemote.LogMessage("Socket connection to Pioneer receiver wrote data", WifiRemote.LogType.Debug);
            socket.Read(AsyncSocket.CRLFData, -1, 0);
        }

        void socket_DidRead(AsyncSocket sender, byte[] data, long tag)
        {
            String response = Encoding.ASCII.GetString(data);
            WifiRemote.LogMessage("Socket connection to Pioneer receiver read data: " + response, WifiRemote.LogType.Debug);
            socket.Read(AsyncSocket.CRLFData, -1, 0);
        }
        #endregion
    }
}
