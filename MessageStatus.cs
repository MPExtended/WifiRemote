using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;
using MediaPortal.GUI.Library;

namespace WifiRemote
{
    class MessageStatus
    {
        string type = "status";

        public string Type
        {
            get { return type; }
        }

        public bool IsPlaying
        {
            get { return g_Player.Playing; }
        }

        public bool IsPaused
        {
            get { return g_Player.Paused; }
        }


        /// <summary>
        /// Media title
        /// </summary>
        public string Title 
        {
            get 
            {
                try
                {
                    return GUIPropertyManager.GetProperty("#Play.Current.Title");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Currently active module
        /// </summary>
        public string CurrentModule
        {
            get 
            {
                try
                {
                    return GUIPropertyManager.GetProperty("#currentmodule");
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Currently selected GUI item label
        /// </summary>
        public string SelectedItem
        {
            get 
            {
                // The currently selected item may hide in the property 
                // #selecteditem or #highlightedbutton.
                string selected = "";
                try
                {
                    selected = GUIPropertyManager.GetProperty("#selecteditem");
                    if (selected.Equals(String.Empty))
                    {
                        selected = GUIPropertyManager.GetProperty("#highlightedbutton");
                    }
                }
                catch (Exception) {}

                return selected;
            }
        }
        
        /// <summary>
        /// Contructor.
        /// </summary>
        public MessageStatus()
        {
        
        }
    }
}
