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
        /// <summary>
        /// The localized name of "fullscreen".
        /// Used to detect if the player is in fullscreen
        /// mode without any dialog on top.
        /// </summary>
        private String localizedFullscreen;
        private String localizedFullscreenVideo;
        private String localizedFullscreenTV;
        private String localizedFullscreenMusic;

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

        /// <summary>
        /// <code>true</code> if g_Play is in fullscreen and on top
        /// </summary>
        public bool IsFullscreen
        {
            get 
            { 
                return (currentModule == localizedFullscreen ||
                        currentModule == localizedFullscreenMusic ||
                        currentModule == localizedFullscreenTV ||
                        currentModule == localizedFullscreenVideo); 
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
            localizedFullscreen = GUILocalizeStrings.Get(595);
            localizedFullscreenTV = GUILocalizeStrings.Get(100000 + (int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
            localizedFullscreenVideo = GUILocalizeStrings.Get(100000 + (int)GUIWindow.Window.WINDOW_FULLSCREEN_VIDEO);
            localizedFullscreenMusic = GUILocalizeStrings.Get(100000 + (int)GUIWindow.Window.WINDOW_FULLSCREEN_MUSIC);
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
