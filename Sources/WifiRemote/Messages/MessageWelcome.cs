using System;
using System.Collections.Generic;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using System.Collections;
using System.Linq;

namespace WifiRemote
{
    class MessageWelcome : IMessage
    {
        string type = "welcome";
        int server_version = 5;
        AuthMethod authMethod = AuthMethod.UserPassword;

        /// <summary>
        /// Type of this method
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// API version of this WifiRemote instance. 
        /// Should be increased on breaking changes.
        /// </summary>
        public int Server_Version 
        { 
            get { return server_version; }
        }

        /// <summary>
        /// Authentication method required of the client.
        /// </summary>
        public AuthMethod AuthMethod
        {
            get { return authMethod; }
            set { authMethod = value; }
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        public MessageWelcome() {}
    }
}
