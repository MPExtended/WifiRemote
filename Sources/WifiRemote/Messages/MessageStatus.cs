using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;
using MediaPortal.GUI.Library;

namespace WifiRemote
{
    /// <summary>
    /// Contains status information about the MediaPortal instance.
    /// </summary>
    class MessageStatus : IMessage
    {
        public string Type
        {
            get { return "status"; }
        }

        private bool isPlaying;
        /// <summary>
        /// <code>true</code> if MediaPortal is playing a file
        /// </summary>
        public bool IsPlaying
        {
            get 
            {
                isPlaying = g_Player.Playing;
                return isPlaying; 
            }
        }

        private bool isPaused;
        /// <summary>
        /// <code>true</code> if MediaPortal is playing a file but it's paused
        /// </summary>
        public bool IsPaused
        {
            get 
            {
                isPaused = g_Player.Paused;
                return isPaused; 
            }
        }

        private string title;
        /// <summary>
        /// Media title
        /// </summary>
        public string Title 
        {
            get 
            {
                try
                {
                    title = GUIPropertyManager.GetProperty("#Play.Current.Title");
                    return title;
                }
                catch (Exception)
                {
                    title = "";
                    return "";
                }
            }
        }

        private string currentModule;
        /// <summary>
        /// Currently active module
        /// </summary>
        public string CurrentModule
        {
            get 
            {
                try
                {
                    currentModule = GUIPropertyManager.GetProperty("#currentmodule");
                    return currentModule;
                }
                catch (Exception)
                {
                    currentModule = "";
                    return "";
                }
            }
        }

        private string selectedItem;
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

                selectedItem = selected;
                return selected;
            }
        }

        
        /// <summary>
        /// Contructor.
        /// </summary>
        public MessageStatus()
        {
        
        }


        /// <summary>
        /// Checks if the status message has changed since 
        /// the last call.
        /// </summary>
        /// <returns>true if the status has changed, false otherwise</returns>
        public bool IsChanged()
        {
            return (isPlaying != IsPlaying
                || isPaused != IsPaused
                || title != Title
                || currentModule != CurrentModule
                || selectedItem != SelectedItem);
        }
    }
}
