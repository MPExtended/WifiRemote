using System;
using System.Collections.Generic;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using System.Collections;
using System.Linq;

namespace WifiRemote
{
    /// <summary>
    /// Message containing all plugins installed on the htpc
    /// </summary>
    class MessagePlugins
    {
        string type = "plugins";
        public string Type
        {
            get { return type; }
        }

        ArrayList plugins;
        /// <summary>
        /// A list of installed and active window plugins.
        /// </summary>
        public ArrayList Plugins
        {
            get { return plugins; }
        }

        /// <summary>
        /// Contructor.
        /// </summary>
        public MessagePlugins()
        {
            plugins = WifiRemote.GetActiveWindowPluginsAndIDs();
        }
    }
}
