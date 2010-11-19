using System;
using System.Collections.Generic;
using MediaPortal.GUI.Library;

namespace WifiRemote
{
    class MessageWelcome
    {
        string type = "welcome";
        int server_version = 1;
        Dictionary<String, int> plugins;
        MessageStatus status;
        MessageVolume volume;

        public string Type
        {
            get { return type; }
        }

        public int Server_Version 
        { 
            get { return server_version; }
        }

        public Dictionary<String, int> Plugins
        {
            get { return plugins; }
        }

        public MessageStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public MessageVolume Volume
        {
            get { return volume; }
            set { volume = value; }
        }


        /// <summary>
        /// Contructor.
        /// </summary>
        public MessageWelcome()
        {
            plugins = new Dictionary<string, int>();
            getActiveWindowPluginsAndIDs();
        }

        /// <summary>
        /// Get all active window plugins and the corresponding window IDs.
        /// This can be used in the client to jump to a specific plugin.
        /// </summary>
        private void getActiveWindowPluginsAndIDs()
        {
            foreach (ISetupForm plugin in PluginManager.SetupForms)
            {
                if (plugin.GetWindowId() != -1)
                {
                    plugins.Add(plugin.PluginName(), plugin.GetWindowId());
                }
            }
        }
    }
}
