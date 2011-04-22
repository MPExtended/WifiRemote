using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deusty.Net;

namespace WifiRemote
{
    public class RemoteClient
    {
        /// <summary>
        /// The socket that handles communication
        /// </summary>
        public AsyncSocket Socket { get; set; }

        /// <summary>
        /// List of properties to which this client has subscribed. If any of these properties changes,
        /// the client will get notified
        /// </summary>
        public List<String> Properties { get; set; }

        /// <summary>
        /// Username for client authentification
        /// </summary>
        public String User { get; set; }

        /// <summary>
        /// Password for client authentification
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Passcode for client authentification
        /// </summary>
        public String PassCode { get; set; }

        /// <summary>
        /// By which method did this client login
        /// </summary>
        public AuthMethod AuthenticatedBy { get; set; }

        /// <summary>
        /// Is the client already authentificated
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Name of the client
        /// </summary>
        public String ClientName { get; set; }

        public RemoteClient()
        {
            ClientName = "Unknown";
        }       
    }
}
