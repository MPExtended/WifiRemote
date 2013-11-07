using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.Utility
{
    class NotificationCenter
    {
        private static volatile NotificationCenter instance;
        private static object syncRoot = new Object();

        private NotificationCenter() { }


        public delegate void ClientConnectedHandler();
        public event ClientConnectedHandler ClientConnected = delegate { };

        public delegate void ClientDisconnectedHandler();
        public event ClientDisconnectedHandler ClientDisconnected = delegate { };

        public delegate void AllClientsDisconnectedHandler();
        public event AllClientsDisconnectedHandler AllClientsDisconnected = delegate { };

        /// <summary>
        /// Returns the singleton instance.
        /// </summary>
        public static NotificationCenter Instance
        {
            get
            {
                if (instance == null) 
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new NotificationCenter();
                        }
                    }
                }

                return instance;
            }
        }

        #region Notifications
        /// <summary>
        /// A client connected to WifiRemote
        /// </summary>
        public void postClientConnectedNotification()
        {
            ClientConnected();
        }

        /// <summary>
        /// A client disconnected from WifiRemote
        /// </summary>
        public void postClientDisconnectedNotification()
        {
            ClientDisconnected();
        }

        /// <summary>
        /// All clients disconnected from WifiRemote
        /// </summary>
        public void postAllClientsDisconnectedNotification()
        {
            AllClientsDisconnected();
        }

        #endregion
    }
}
