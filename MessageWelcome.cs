using System;
using System.Collections.Generic;
using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using System.Collections;

namespace WifiRemote
{
    class MessageWelcome
    {
        string type = "welcome";
        int server_version = 1;
        ArrayList plugins;
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

        public ArrayList Plugins
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
            plugins = new ArrayList();
            getActiveWindowPluginsAndIDs();
        }

        /// <summary>
        /// Get all active window plugins and the corresponding window IDs.
        /// This can be used in the client to jump to a specific plugin.
        /// 
        /// We are also sending the plugin icon as byte array if it exists.
        /// </summary>
        private void getActiveWindowPluginsAndIDs()
        {
            foreach (ISetupForm plugin in PluginManager.SetupForms)
            {
                if (plugin.GetWindowId() != -1)
                {
                    byte[] iconBytes = new byte[0];

                    // Load plugin icon
                    Type pluginType = plugin.GetType();
                    PluginIconsAttribute[] icons = (PluginIconsAttribute[])pluginType.GetCustomAttributes(typeof(PluginIconsAttribute), false);
                    if (icons.Length > 0)
                    {
                        string resourceName = icons[0].ActivatedResourceName;
                        if (!string.IsNullOrEmpty(resourceName))
                        {
                            System.Drawing.Image icon = null;
                            try
                            {
                                icon = System.Drawing.Image.FromStream(pluginType.Assembly.GetManifestResourceStream(resourceName));
                            }
                            catch (Exception e)
                            {
                                WifiRemote.LogMessage("Could not load plugin icon: " + e.Message, WifiRemote.LogType.Error);
                            }

                            if (icon != null)
                            {
                                iconBytes = WifiRemote.imageToByteArray(icon, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }
                    }

                    plugins.Add(new WindowPlugin(plugin.PluginName(), plugin.GetWindowId(), iconBytes));
                }
            }
        }
    }
}
