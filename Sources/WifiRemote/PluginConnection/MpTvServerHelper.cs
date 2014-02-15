using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using TvDatabase;
using MediaPortal.Util;
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
        protected delegate void PlayRadioChannelDelegate(int channelId);

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
                GUIGraphicsContext.form.Invoke(d, recordingId, startPos, startFullscreen);
                return;
            }

            TvDatabase.Recording rec = TvDatabase.Recording.Retrieve(recordingId);

            bool success = TvPlugin.TVUtil.PlayRecording(rec, startPos);

            if (startFullscreen && success)
                {
                    WifiRemote.LogMessage("Switching to fullscreen", WifiRemote.LogType.Debug);
                    g_Player.ShowFullScreenWindowTV();
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

            if (g_Player.Playing && !g_Player.IsTimeShifting)
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
                    g_Player.ShowFullScreenWindowTV();
                }
            }
            else
            {
                Log.Warn("Couldn't retrieve channel for id: " + channelId);
            }
        }

        /// <summary>
        /// Play a radio channel
        /// </summary>
        /// <param name="channelId">Id of the channel</param>
        public static void PlayRadioChannel(int channelId)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlayRadioChannelDelegate d = PlayRadioChannel;
                GUIGraphicsContext.form.Invoke(d, channelId);
                return;
            }

            WifiRemote.LogMessage("Start radio channel " + channelId, WifiRemote.LogType.Debug);
            TvDatabase.Channel channel = TvDatabase.Channel.Retrieve(channelId);

            if (channel != null)
            {
                if (GUIWindowManager.ActiveWindow != (int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_RADIO)
                {
                    WifiRemote.LogMessage("Radio Window not active, activating it", WifiRemote.LogType.Debug);
                    MediaPortal.GUI.Library.GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_RADIO);
                }

                GUIPropertyManager.RemovePlayerProperties();
                GUIPropertyManager.SetProperty("#Play.Current.ArtistThumb", channel.DisplayName);
                GUIPropertyManager.SetProperty("#Play.Current.Album", channel.DisplayName);
                GUIPropertyManager.SetProperty("#Play.Current.Title", channel.DisplayName);

                GUIPropertyManager.SetProperty("#Play.Current.Title", channel.DisplayName);
                string strLogo = Utils.GetCoverArt(Thumbs.Radio, channel.DisplayName);
                if (string.IsNullOrEmpty(strLogo))
                {
                    strLogo = "defaultMyRadioBig.png";
                }
                GUIPropertyManager.SetProperty("#Play.Current.Thumb", strLogo);

                if (g_Player.Playing && !channel.IsWebstream())
                {
                    if (!g_Player.IsTimeShifting || (g_Player.IsTimeShifting && channel.IsWebstream()))
                    {
                        WifiRemote.LogMessage("Stopping current media so we can start playing radio", WifiRemote.LogType.Debug);
                        g_Player.Stop();
                    }
                }
                bool success = false;
                if (channel.IsWebstream())
                {
                    IList<TuningDetail> details = channel.ReferringTuningDetail();
                    TuningDetail detail = details[0];
                    WifiRemote.LogMessage("Play webStream:" +detail.Name +  ", url:" + detail.Url, WifiRemote.LogType.Debug);
                    success = g_Player.PlayAudioStream(detail.Url);
                    GUIPropertyManager.SetProperty("#Play.Current.Title", channel.DisplayName);
                }
                else
                {
                    // TV card radio channel
                    WifiRemote.LogMessage("Play TV card radio channel", WifiRemote.LogType.Debug);
                    //Check if same channel is alrady playing
                    if (g_Player.IsRadio && g_Player.Playing)
                    {
                        Channel currentlyPlaying = TvPlugin.TVHome.Navigator.Channel;
                        if (currentlyPlaying != null && currentlyPlaying.IdChannel == channel.IdChannel)
                        {
                            WifiRemote.LogMessage("Already playing TV card radio channel with id:" + channel.IdChannel + ", do not tune again", WifiRemote.LogType.Debug);
                        }
                        else
                        {
                            success = TvPlugin.TVHome.ViewChannelAndCheck(channel);
                        }
                    }
                    else
                    {
                        success = TvPlugin.TVHome.ViewChannelAndCheck(channel);
                    }   
                }                 
                WifiRemote.LogMessage("Started radio channel " + channelId + " Success: " + success, WifiRemote.LogType.Debug);
            }
            else
            {
                Log.Warn("Couldn't retrieve radio channel for id: " + channelId);
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

        internal static NowPlayingTv GetNowPlayingTv()
        {
            NowPlayingTv tv = new NowPlayingTv();
            return tv;
        }

        internal static NowPlayingRecording GetNowPlayingRecording()
        {
            NowPlayingRecording recording = new NowPlayingRecording(g_Player.Player.CurrentFile);
            return recording;
        }

        internal static NowPlayingRadio GetNowPlayingRadio()
        {
            NowPlayingRadio radio = new NowPlayingRadio();
            return radio;
        }

        internal static TvDatabase.Channel GetCurrentTimeShiftingTVChannel()
        {
            if (TVHome.Connected && TVHome.Card.IsTimeShifting)
            {
                 int id = TVHome.Card.IdChannel;
                 if (id >= 0)
                 {
                     TvDatabase.Channel current = TvDatabase.Channel.Retrieve(id);
                     return current;
                 }
            }    
            return null;
        }
        
    }
}
