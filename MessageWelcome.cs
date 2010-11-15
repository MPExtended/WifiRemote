using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MediaPortal.Configuration;

namespace WifiRemote
{
    class MessageWelcome
    {
        string type = "welcome";
        int server_version = 1;
        List<String> plugins;
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

        public List<String> Plugins
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
            readActivePlugins();
        }

        /// <summary>
        /// Read all active plugins from the MediaPortal.xml
        /// Those plugin names are sent in the welcome message. 
        /// The client can choose to support those plugins (by
        /// providing shortcuts to selected plugins, for example).
        /// </summary>
        private void readActivePlugins() 
        {
            plugins = new List<string>();
            bool inPluginSection = false;
            String pluginName = String.Empty;

            try
            {
                XmlTextReader reader = new XmlTextReader(Config.GetFile(Config.Dir.Config, "MediaPortal.xml"));

                while (reader.Read())
                {
                    // Look for <section name="plugins">
                    if (!inPluginSection)
                    {
                        if (reader.NodeType == XmlNodeType.Element
                            && reader.Name == "section"
                            && reader.AttributeCount == 1
                            && reader.GetAttribute(0) == "plugins")
                        {
                            inPluginSection = true;
                        }
                    }
                    else
                    {
                        // End of <section name="plugins">
                        if (reader.NodeType == XmlNodeType.EndElement
                            && reader.Name == "section")
                        {
                            inPluginSection = false;
                        }
                        // Every plugin has an "entry" ...
                        else if (reader.NodeType == XmlNodeType.Element
                            && reader.HasAttributes
                            && reader.Name == "entry")
                        {
                            pluginName = reader.GetAttribute(0);
                        }
                        // ... that we only add to the list if it is active (yes)
                        else if (reader.NodeType == XmlNodeType.Text
                            && pluginName != String.Empty
                            && reader.Value == "yes")
                        {
                            plugins.Add(pluginName);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // XML Text Reader Exception

            }
        }
    }
}
