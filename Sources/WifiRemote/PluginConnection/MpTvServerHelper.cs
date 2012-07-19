using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using TvPlugin;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Helper class for MP TvServer actions
    /// </summary>
    public class MpTvServerHelper
    {
        protected delegate void PlayRecordingDelegate(int recordingId, int startPos, bool startFullscreen);
        protected delegate void PlayTvChannelDelegate(int channelId, bool startFullscreen);

        /// <summary>
        /// Plays a recording from mp tvserver
        /// </summary>
        /// <param name="id">Id of recording</param>
        /// <param name="startPos">Start postion</param>
        /// <param name="startFullscreen">If true, will switch to fullscreen after playback is started</param>
        public static void PlayRecording(int recordingId, int startPos, bool startFullscreen)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlayRecordingDelegate d = PlayRecording;
                GUIGraphicsContext.form.Invoke(d, recordingId, startPos);
                return;
            }

            TvDatabase.Recording rec = TvDatabase.Recording.Retrieve(recordingId);

            bool success = TvPlugin.TVUtil.PlayRecording(rec, startPos);

            if (startFullscreen && success)
            {
                WifiRemote.LogMessage("Switching to fullscreen", WifiRemote.LogType.Debug);
                g_Player.ShowFullScreenWindow();
            }
        }


        /// <summary>
        /// Play a tv channel
        /// </summary>
        /// <param name="channelId">Id of the channel</param>
        /// <param name="startFullscreen">If true, will switch to fullscreen after playback is started</param>
        public static void PlayTvChannel(int channelId, bool startFullscreen)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlayTvChannelDelegate d = PlayTvChannel;
                GUIGraphicsContext.form.Invoke(d, channelId, startFullscreen);
                return;
            }

            if (g_Player.Playing && !g_Player.IsTV)
            {
                WifiRemote.LogMessage("Stopping current media so we can start playing tv", WifiRemote.LogType.Debug);
                g_Player.Stop();
            }

            //the tv window isn't active and we're not playing tv fullscreen
            //if (GUIWindowManager.ActiveWindow != (int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_TV &&
            if(!g_Player.Playing)
            {
                WifiRemote.LogMessage("Tv Window not active, activating it", WifiRemote.LogType.Debug);
                MediaPortal.GUI.Library.GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_TV);
            }

            WifiRemote.LogMessage("Start channel " + channelId, WifiRemote.LogType.Debug);
            TvDatabase.Channel channel = TvDatabase.Channel.Retrieve(channelId);
            if (channel != null)
            {
                bool success = TvPlugin.TVHome.ViewChannelAndCheck(channel);
                WifiRemote.LogMessage("Started channel " + channelId + " Success: " + success, WifiRemote.LogType.Info);
                if (startFullscreen && success)
                {
                    WifiRemote.LogMessage("Switching to fullscreen", WifiRemote.LogType.Debug);
                    g_Player.ShowFullScreenWindow();
                }
            }
            else
            {
                Log.Warn("Couldn't retrieve channel for id: " + channelId);
            }
        }

        /// <summary>
        /// Show the details of a tv recording in MediaPortal
        /// </summary>
        /// <param name="recordingId">Id of recording</param>
        internal static void ShowRecordingDetails(int recordingId)
        {
            
            WifiRemote.LogMessage("Not implemented yet for mp recording", WifiRemote.LogType.Info);
        }


        /// <summary>
        /// Show the details page of a tv channel
        /// </summary>
        /// <param name="tvChannelId">Id of channel</param>
        internal static void ShowTvChannelDetails(int tvChannelId)
        {
            WifiRemote.LogMessage("Not implemented yet for mp tv channels", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Create a playlist item from a MP recording
        /// </summary>
        /// <param name="recordingId">Id of recording</param>
        /// <returns>Playlist item</returns>
        internal static MediaPortal.Playlists.PlayListItem CreatePlaylistItemFromRecording(int recordingId)
        {
            WifiRemote.LogMessage("Not implemented yet for mp tv recordings", WifiRemote.LogType.Info);

            return null;
        }
    }
}
