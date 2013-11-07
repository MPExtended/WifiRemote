using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deusty.Net;
using WifiRemote.Utility;

namespace WifiRemote.HardwareController.AudioController
{
    /// <summary>
    /// Audio hardware controller for network enabled 
    /// Pioneer VSX 921/1021 AV receivers.
    /// 
    /// Pioneer remote command docs can be found here:
    /// http://www.pioneerelectronics.com/StaticFiles/PUSA/Files/Home%20Custom%20Install/VSX-1120-K-RS232.PDF
    /// </summary>
    class PioneerVSXAudioController : AbstractAudioController
    {
        private AsyncSocket _socket;
        private List<String> _commandQueue;
        private String _ip;
        private ushort _port;
        private String _volume;
        private bool _muted;
        private bool _powered;
        
        /// <summary>
        /// Creates a new PioneerVSXAudioController.
        /// 
        /// Different models use different ports.
        /// VSX 42/822/921/1021: 8102
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public PioneerVSXAudioController(String ip, ushort port)
        {
            _ip = ip;
            _port = port;
            _commandQueue = new List<string>();

            // Get receiver status
            // Setting defaults to -1 volume, not muted and power on
            _volume = "VU101";
            _muted = false;
            _powered = true;


            // Observe client connected/disconnected notifications so that we
            // can connect and disconnect to the receiver. There can only be
            // one active network connection to the receiver, so we don't want
            // to block that when no client is connected to WifiRemote.
            NotificationCenter.Instance.AllClientsDisconnected += new NotificationCenter.AllClientsDisconnectedHandler(AllClientsDisconnected);
            NotificationCenter.Instance.ClientConnected += new NotificationCenter.ClientConnectedHandler(ClientConnected);
        }

        private void createSocket()
        {
            _socket = new AsyncSocket();
            _socket.DidRead += new AsyncSocket.SocketDidRead(socket_DidRead);
            _socket.DidWrite += new AsyncSocket.SocketDidWrite(socket_DidWrite);
            _socket.DidConnect += new AsyncSocket.SocketDidConnect(socket_DidConnect);
            _socket.DidClose += new AsyncSocket.SocketDidClose(socket_DidClose);
        }

        private void sendCommand(String command)
        {
            // Prepare command for sending
            command = command + "\r";

            // Create socket
            if (_socket == null) 
            {
                createSocket();
            }

            // Check if connected to receiver
            if (!_socket.SmartConnected)
            {
                // Check if the receiver is on
                // (We can connect if network power is enabled in the receiver setup menu)
                // If the receiver is off, switch it on first
                if (!_powered)
                {
                    _commandQueue.Add("PO");
                }

                // Add command to queue for later execution
                _commandQueue.Add(command);

                Exception error;
                if (!_socket.Connect(_ip, _port, out error))
                {
                    WifiRemote.LogMessage("Failed to connect to Pioneer receiver: " + error.Message, WifiRemote.LogType.Error);
                }
            }
            else
            {
                WifiRemote.LogMessage("Sending command to Pioneer: " + command, WifiRemote.LogType.Debug);
                byte[] data = Encoding.ASCII.GetBytes(command);
                _socket.Write(data, -1, 0);
            }
        }

        private void prepareReceiverConnection()
        {
            // ... current volume
            sendCommand("?V");

            // ... muted?
            sendCommand("?M");

            // ... switched on?
            sendCommand("?P");
        }


        #region IAudioController Methods
        public override void VolumeUp()
        {
            sendCommand("VU");
        }

        public override void VolumeDown()
        {
            sendCommand("VD");
        }

        public override void ToggleMute()
        {
            sendCommand("MZ");
        }

        public override int Volume()
        {
            try
            {
                int volumeValue = (_volume.Length == 6) ? Int32.Parse(_volume.Substring(3, 3)) : 0;
                // Volume on Pioneer receivers seems to range from 0 (muted, one step below -80.0db) to 185 (+12.0db)
                int volumeInPercent = (volumeValue == 0) ? 0 : (int)Math.Round(volumeValue / 1.85);
                return volumeInPercent;
            }
            catch (Exception e)
            {
                WifiRemote.LogMessage("Pioneer receiver delivered invalid volume: " + _volume + " with error " + e.Message, WifiRemote.LogType.Error);
                return -1;
            }
        }

        public override bool IsMuted()
        {
            return _muted;
        }

        /// <summary>
        /// Sets the volume of the client
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        /// <param name="relative">True if the volume should be changed relative to the current volume</param>
        public override void SetVolume(int volume, bool relative)
        {
            if (relative)
            {
                volume = Volume() + volume;
                volume = Math.Max(0, Math.Min(volume, 100));
            }

            // transform percent to pioneer volume
            int volumeValue = (int)Math.Max(0, Math.Min(Math.Round(volume * 1.85), 185.0));
            WifiRemote.LogMessage("Setting Pioneer volume to " + volumeValue.ToString("000"), WifiRemote.LogType.Debug);
            sendCommand(volumeValue.ToString("000") + "VL");

            _volume = "VOL" + volumeValue.ToString("000");
        }
        #endregion



        #region Socket events
        void socket_DidClose(AsyncSocket sender)
        {
            _socket = null;
            WifiRemote.LogMessage("Socket connection to Pioneer receiver closed", WifiRemote.LogType.Debug);
        }

        void socket_DidConnect(AsyncSocket sender, System.Net.IPAddress address, ushort port)
        {
            foreach (String command in _commandQueue)
            {
                WifiRemote.LogMessage("Sending command from queue to Pioneer: " + command, WifiRemote.LogType.Debug);
                byte[] data = Encoding.ASCII.GetBytes(command);
                _socket.Write(data, -1, 0);
            }

            _commandQueue.Clear();
        }

        void socket_DidWrite(AsyncSocket sender, long tag)
        {
            WifiRemote.LogMessage("Socket connection to Pioneer receiver wrote data", WifiRemote.LogType.Debug);
            _socket.Read(AsyncSocket.CRLFData, -1, 0);
        }

        void socket_DidRead(AsyncSocket sender, byte[] data, long tag)
        {
            String response = Encoding.ASCII.GetString(data);
            if (response.StartsWith("MUT"))
            {
                _muted = response.StartsWith("MUT0");
                OnAudioControllerStatusChanged(EventArgs.Empty);
            }
            else if (response.StartsWith("VOL"))
            {
                _volume = response.TrimEnd('\r', '\n');
                OnAudioControllerStatusChanged(EventArgs.Empty);
            }
            else if (response.StartsWith("PWR"))
            {
                _powered = response.StartsWith("PWR0");
            }

            WifiRemote.LogMessage("Socket connection to Pioneer receiver read data: " + response, WifiRemote.LogType.Debug);
            _socket.Read(AsyncSocket.CRLFData, -1, 0);
        }
        #endregion

        #region Notification observers
        /// <summary>
        /// All clients disconnected. Shut down receiver connection.
        /// </summary>
        void AllClientsDisconnected()
        {
            WifiRemote.LogMessage("Last client disconnected, disconnecting from receiver.", WifiRemote.LogType.Debug);
            _socket.CloseAfterWriting();
        }

        /// <summary>
        /// A new user connected. Establish connection to AV receiver
        /// if it isn't established already.
        /// </summary>
        void ClientConnected()
        {
            WifiRemote.LogMessage("Client connected, checking receiver connection ...", WifiRemote.LogType.Debug);
            if (_socket == null || !_socket.SmartConnected)
            {
                WifiRemote.LogMessage("... connecting to receiver!", WifiRemote.LogType.Debug);
                prepareReceiverConnection();
            }
        }
        #endregion
    }
}