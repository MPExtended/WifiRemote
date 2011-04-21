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
        int server_version = 3;

        public string Type
        {
            get { return type; }
        }

        public int Server_Version 
        { 
            get { return server_version; }
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        public MessageWelcome() {}
    }
}
