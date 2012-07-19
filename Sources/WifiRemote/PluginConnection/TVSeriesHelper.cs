using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowPlugins.GUITVSeries;
using MediaPortal.GUI.Library;
using System.Threading;
using MediaPortal.Player;
using WifiRemote.PluginConnection;

namespace WifiRemote
{
    /// <summary>
    /// Helper class for MpTvSeries actions
    /// </summary>
    class TVSeriesHelper
    {
        static VideoHandler player = null;
        static PlayListPlayer playlistPlayer;
        protected delegate void PlayEpisodeAsyncDelegate(DBEpisode episode, bool resume, int startPosition);
        protected delegate void PlaySeasonAsyncDelegate(int seriesId, int seasonNumber, bool autostart, int offset, bool onlyUnwatched, bool switchToPlaylistView);
        protected delegate void PlaySeriesAsyncDelegate(int seriesId, bool autostart, int offset, bool onlyUnwatched, bool switchToPlaylistView);

        /// <summary>
        /// Get a series id by show name
        /// </summary>
        /// <param name="seriesName">Name of the series to look for</param>
        /// <returns>A series id or null if none was found</returns>
        public static int? GetSeriesIdByName(string seriesName)
        {
            SQLCondition conditions = new SQLCondition();
            conditions.Add(new DBOnlineSeries(), DBOnlineSeries.cPrettyName, seriesName, SQLConditionType.Like);
            List<DBSeries> seriesList = DBSeries.Get(conditions);

            // Return best matching series or null if no result was found
            if (seriesList.Count == 1)
            {
                return seriesList[0][DBOnlineSeries.cID];
            }
            else if (seriesList.Count > 1)
            {
                foreach (DBSeries series in seriesList)
                {
                    if (series[DBOnlineSeries.cPrettyName].Equals(seriesName))
                    {
                        return series[DBOnlineSeries.cID];
                    }
                }

                return seriesList[0][DBOnlineSeries.cID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Playback the first unwatched episode for a series using TVSeries internal Video Handler
        /// If no Unwatched episodes exists, play the Most Recently Aired
        /// 
        /// Taken from Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="seriesid">series id of episode</param>
        /// <param name="resume">Resume from last stop?</param>
        public static void PlayFirstUnwatchedEpisode(int seriesid, bool resume)
        {
            var episodes = DBEpisode.Get(seriesid);
            if (episodes == null || episodes.Count == 0) return;

            // filter out anything we can't play
            episodes.RemoveAll(e => string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episodes.Count == 0) return;

            // sort episodes using DBEpisode sort comparer
            // this takes into consideration Aired/DVD order and Specials in-line sorting
            episodes.Sort();

            // get first episode unwatched, otherwise get most recently aired
            var episode = episodes.Where(e => e[DBOnlineEpisode.cWatched] == 0).FirstOrDefault();
            if (episode == null)
            {
                WifiRemote.LogMessage("No Unwatched episodes found, Playing most recent episode", WifiRemote.LogType.Info);
                episode = episodes.LastOrDefault();
            }

            if (episode != null)
            {
                PlayEpisode(episode, resume);
            }
        }

        /// <summary>
        /// Play a random episode of a series
        /// </summary>
        /// <param name="seriesId">ID of a series</param>
        /// <param name="resume">Resume from last stop?</param>
        public static void PlayRandomEpisode(int seriesId, bool resume)
        {
            List<DBEpisode> episodes = DBEpisode.Get(seriesId);
            if (episodes == null || episodes.Count == 0) return;

            // filter out anything we can't play
            episodes.RemoveAll(e => string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episodes.Count == 0) return;


            DBEpisode episode = episodes.GetRandomElement<DBEpisode>();
            if (episode != null)
            {
                PlayEpisode(episode, resume);
            }
        }

        /// <summary>
        /// Play all episodes of a season
        /// </summary>
        /// <param name="seriesId">ID of a series</param>
        /// <param name="seasonNumber">Number of the season</param>
        /// <param name="onlyUnwatched">Play only unwatched episodes</param>
        /// <param name="autostart">If yes, automatically starts playback with the first episode</param>
        /// <param name="startIndex">Index of the item with which playback should start</param>
        /// <param name="switchToPlaylistView">If yes the playlistview will be shown</param>
        public static void PlaySeason(int seriesId, int seasonNumber, bool autostart, int startIndex, bool onlyUnwatched, bool switchToPlaylistView)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlaySeasonAsyncDelegate d = new PlaySeasonAsyncDelegate(PlaySeason);
                GUIGraphicsContext.form.Invoke(d, new object[] { seriesId, seasonNumber, autostart, startIndex, onlyUnwatched, switchToPlaylistView });
                return;
            }

            List<DBEpisode> episodes = DBEpisode.Get(seriesId, seasonNumber);
            if (episodes == null || episodes.Count == 0) return;

            // filter out anything we can't play
            episodes.RemoveAll(e => string.IsNullOrEmpty(e[DBEpisode.cFilename]));

            // filter out watched episodes
            if (onlyUnwatched)
            {
                episodes.RemoveAll(e => e[DBOnlineEpisode.cWatched] != 0);
            }
            if (episodes.Count == 0) return;

            // Sort episodes and add them to the MP-TVSeries playlist player
            // Setup playlist player
            if (playlistPlayer == null)
            {
                playlistPlayer = PlayListPlayer.SingletonPlayer;
                playlistPlayer.PlaylistAutoPlay = true;
                playlistPlayer.RepeatPlaylist = DBOption.GetOptions(DBOption.cRepeatPlaylist);
            }

            playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Clear();
            episodes.Sort();

            foreach (DBEpisode episode in episodes)
            {
                PlayListItem playlistItem = new PlayListItem(episode);
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Add(playlistItem);
            }

            //automatically start playing the playlist
            if (autostart)
            {
                // and activate the playlist window if its not activated yet
                if (switchToPlaylistView)
                {
                    GUIWindowManager.ActivateWindow(GUITVSeriesPlayList.GetWindowID);
                }

                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                playlistPlayer.Reset();
                playlistPlayer.Play(0);
            }
        }

        /// <summary>
        /// Play all episodes of a series
        /// </summary>
        /// <param name="seriesId">ID of a series</param>
        /// <param name="onlyUnwatched">Play only unwatched episodes</param>
        /// <param name="autostart">If yes, automatically starts playback with the first episode</param>
        /// <param name="startIndex">Index of the item with which playback should start</param>
        /// <param name="switchToPlaylistView">If yes the playlistview will be shown</param>
        public static void PlaySeries(int seriesId, bool autostart, int startIndex, bool onlyUnwatched, bool switchToPlaylistView)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlaySeriesAsyncDelegate d = new PlaySeriesAsyncDelegate(PlaySeries);
                GUIGraphicsContext.form.Invoke(d, new object[] { seriesId, autostart, startIndex, onlyUnwatched, switchToPlaylistView });
                return;
            }

            List<DBEpisode> episodes = DBEpisode.Get(seriesId);
            if (episodes == null || episodes.Count == 0) return;

            // filter out anything we can't play
            episodes.RemoveAll(e => string.IsNullOrEmpty(e[DBEpisode.cFilename]));

            // filter out watched episodes
            if (onlyUnwatched)
            {
                episodes.RemoveAll(e => e[DBOnlineEpisode.cWatched] != 0);
            }
            if (episodes.Count == 0) return;

            // Sort episodes and add them to the MP-TVSeries playlist player
            // Setup playlist player
            if (playlistPlayer == null)
            {
                playlistPlayer = PlayListPlayer.SingletonPlayer;
                playlistPlayer.PlaylistAutoPlay = true;
                playlistPlayer.RepeatPlaylist = DBOption.GetOptions(DBOption.cRepeatPlaylist);
            }

            playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Clear();
            episodes.Sort();

            foreach (DBEpisode episode in episodes)
            {
                PlayListItem playlistItem = new PlayListItem(episode);
                playlistPlayer.GetPlaylist(PlayListType.PLAYLIST_TVSERIES).Add(playlistItem);
            }

            //automatically start playing the playlist
            if (autostart)
            {
                // and activate the playlist window if its not activated yet
                if (switchToPlaylistView)
                {
                    GUIWindowManager.ActivateWindow(GUITVSeriesPlayList.GetWindowID);
                }

                playlistPlayer.CurrentPlaylistType = PlayListType.PLAYLIST_TVSERIES;
                playlistPlayer.Reset();
                playlistPlayer.Play(0);
            }
        }


        /// <summary>
        /// Play an episode of a specific series and season
        /// 
        /// Thanks to Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="seriesId">ID of the series</param>
        /// <param name="seasonNumber">Number of the season</param>
        /// <param name="episodeNumber">Number of the episode</param>
        /// <paraparam name="resume">Resume from last stop?</paraparam>
        /// <param name="startPosition">Position from which the video should start in seconds (e.g. StartPosition=180 will start the episode 3 minutes into the video). Will be ignored if AskToResume is true.</param>
        public static void Play(int seriesId, int seasonNumer, int episodeNumber, bool resume, int startPosition = 0)
        {
            var episodes = DBEpisode.Get(seriesId, seasonNumer);
            var episode = episodes.FirstOrDefault(e => (e[DBEpisode.cEpisodeIndex] == episodeNumber || e[DBEpisode.cEpisodeIndex2] == episodeNumber) && !string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episode == null) return;

            PlayEpisode(episode, resume, startPosition);
        }
        
        /// <summary>
        /// Play an episode of a specific series and season
        /// 
        /// Thanks to Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="episodeId">ID of the episode</param>
        /// <paraparam name="resume">Resume from last stop?</paraparam>
        /// <param name="startPosition">Position from which the video should start in seconds (e.g. StartPosition=180 will start the episode 3 minutes into the video). Will be ignored if AskToResume is true.</param>
        public static void PlayEpisode(int episodeId, bool resume, int startPosition)
        {
            var episodes = DBEpisode.Get(new SQLCondition(new DBTable("online_episodes"), "EpisodeID", new DBValue(episodeId), SQLConditionType.Equal));
            var episode = episodes.FirstOrDefault(e => !string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episode == null) return;

            PlayEpisode(episode, resume, startPosition);;
        }

        /// <summary>
        /// Play an episode of a specific series and season
        /// 
        /// Thanks to Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="compositeId">Composite id of the episode</param>
        /// <paraparam name="resume">Resume from last stop?</paraparam>
        /// <param name="startPosition">Position from which the video should start in seconds (e.g. StartPosition=180 will start the episode 3 minutes into the video). Will be ignored if AskToResume is true.</param>
        public static void PlayEpisode(String compositeId, bool resume, int startPosition)
        {
            var episodes = DBEpisode.Get(new SQLCondition(new DBTable("online_episodes"), "CompositeID", new DBValue(compositeId), SQLConditionType.Equal));
            var episode = episodes.FirstOrDefault(e => !string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episode == null) return;

            PlayEpisode(episode, resume, startPosition); ;
        }

        /// <summary>
        /// Play an episode
        /// 
        /// Thanks to Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="episode">A valid tvseries episode</param>
        /// <param name="resume">Resume from last stop?</param>
        /// <param name="startPosition">Postion from where playback should be started (only valid if resum=false)</param>
        /// <returns></returns>
        private static void PlayEpisode(DBEpisode episode, bool resume, int startPosition = 0)
        {
            // Play on a new thread
            ThreadStart ts = delegate() { DoPlayEpisode(episode, resume, startPosition); };
            Thread playEpisodeAsync = new Thread(ts);
            playEpisodeAsync.Start();
        }

        /// <summary>
        /// Play episode async
        /// </summary>
        /// <param name="episode">Episode to play</param>
        /// <param name="startPosition">Postion from where playback should be started (only valid if resum=false)</param>
        /// <param name="resume">Resume from last stop?</param>
        private static void DoPlayEpisode(DBEpisode episode, bool resume, int startPosition)
        {

            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlayEpisodeAsyncDelegate d = new PlayEpisodeAsyncDelegate(DoPlayEpisode);
                GUIGraphicsContext.form.Invoke(d, new object[] { episode, resume, startPosition });
                return;
            }

            WifiRemote.LogMessage("Play episode (resume: " + resume + ", pos: " + startPosition, WifiRemote.LogType.Debug);

            if (player == null) player = new VideoHandler();

            // Reset stopTime if resume is false
            if (!resume)
            {
                episode[DBEpisode.cStopTime] = 0;
            }

            player.ResumeOrPlay((DBEpisode)episode);

            if (!resume && startPosition > 0)
            {
                g_Player.SeekAbsolute(startPosition);
            }
        }

        /// <summary>
        /// Is the dialog a series rating dialog
        /// </summary>
        /// <param name="dialog">Dialog</param>
        /// <returns>true if dialog is a pin dialog, false otherwise</returns>
        internal static bool IsTvSeriesRatingDialog(MediaPortal.Dialogs.GUIDialogWindow dialog)
        {
            if (dialog.GetType().Equals(typeof(WindowPlugins.GUITVSeries.GUIUserRating)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Is the dialog a series pin dialog
        /// </summary>
        /// <param name="dialog">Dialog</param>
        /// <returns>true if dialog is a pin dialog, false otherwise</returns>
        internal static bool IsTvSeriesPinDialog(MediaPortal.Dialogs.GUIDialogWindow dialog)
        {
            if (dialog.GetType().Equals(typeof(WindowPlugins.GUITVSeries.GUIPinCode)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Show episode details
        /// </summary>
        /// <param name="seriesId">Id of series</param>
        /// <param name="seasonId">Id of season</param>
        /// <param name="episodeId">Id of episode</param>
        internal static void ShowEpisodeDetails(int seriesId, int seasonId, int episodeId)
        {
            if (seriesId > 0)
            {
                WindowPluginHelper.ActivateWindow(9811, "seriesid:" + seriesId + "|seasonidx:" + seasonId + "|episodeidx:" + episodeId);
            }
        }

        /// <summary>
        /// Show season details
        /// </summary>
        /// <param name="seriesId">Id of series</param>
        /// <param name="seasonId">Id of season</param>
        internal static void ShowSeasonDetails(int seriesId, int seasonId)
        {
            if (seriesId > 0)
            {
                WindowPluginHelper.ActivateWindow(9811, "seriesid:" + seriesId + "|seasonidx:" + seasonId);
            }
        }

        /// <summary>
        /// Show series details
        /// </summary>
        /// <param name="seriesId">Id of series</param>
        internal static void ShowSeriesDetails(int seriesId)
        {
            if (seriesId > 0)
            {
                WindowPluginHelper.ActivateWindow(9811, "seriesid:" + seriesId.ToString());
            }
        }


        /// <summary>
        /// Create PlayListItem from TvSeries episode id
        /// </summary>
        /// <param name="compositeId">Composite id of episode</param>
        /// <returns>PlayListItem item</returns>
        internal static MediaPortal.Playlists.PlayListItem CreatePlaylistItemFromEpisode(String compositeId)
        {
            var episodes = DBEpisode.Get(new SQLCondition(new DBTable("online_episodes"), "CompositeID", new DBValue(compositeId), SQLConditionType.Equal));
            var episode = episodes.FirstOrDefault(e => !string.IsNullOrEmpty(e[DBEpisode.cFilename]));
        
            return CreatePlaylistItemFromEpisode(episode);
        }

        /// <summary>
        /// Create PlayListItem from TvSeries DBEpisode
        /// </summary>
        /// <param name="episode">DBEpisode</param>
        /// <returns>PlayListItem item</returns>
        private static MediaPortal.Playlists.PlayListItem CreatePlaylistItemFromEpisode(DBEpisode episode)
        {
            if (episode != null)
            {
                MediaPortal.Playlists.PlayListItem item = new MediaPortal.Playlists.PlayListItem();
                item.FileName = episode[DBEpisode.cFilename];
                item.Duration = episode[DBEpisode.cLocalPlaytime] / 1000;
                item.Description = episode[DBOnlineEpisode.cEpisodeName];
                return item;
            }
            return null;
        }

        /// <summary>
        /// Create PlayListItem from TvSeries show and season number
        /// </summary>
        /// <param name="seriesId">Id of show</param>
        /// <param name="seasonNumber">Season number</param>
        /// <returns>PlayListItem item</returns>
        internal static List<MediaPortal.Playlists.PlayListItem> CreatePlaylistItemsFromSeason(int seriesId, int seasonNumber)
        {
            List<MediaPortal.Playlists.PlayListItem> returnList = new List<MediaPortal.Playlists.PlayListItem>();
            var episodes = DBEpisode.Get(seriesId, seasonNumber);

            foreach (DBEpisode e in episodes)
            {
                returnList.Add(CreatePlaylistItemFromEpisode(e));
            }

            return returnList;
        }

        /// <summary>
        /// Create PlayListItem from TvSeries show
        /// </summary>
        /// <param name="seriesId">Id of show</param>
        /// <returns>PlayListItem item</returns>
        internal static List<MediaPortal.Playlists.PlayListItem> CreatePlaylistItemsFromShow(int seriesId)
        {
            List<MediaPortal.Playlists.PlayListItem> returnList = new List<MediaPortal.Playlists.PlayListItem>();
            var episodes = DBEpisode.Get(seriesId);

            foreach (DBEpisode e in episodes)
            {
                returnList.Add(CreatePlaylistItemFromEpisode(e));
            }

            return returnList;
        }
    }
}
