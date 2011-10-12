using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowPlugins.GUITVSeries;

namespace WifiRemote
{
    class TVSeriesHelper
    {
        static VideoHandler player = null;

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
        public static void PlayFirstUnwatchedEpisode(int seriesid)
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
                PlayEpisode(episode);
            }
        }

        /// <summary>
        /// Play a random episode of a series
        /// </summary>
        /// <param name="seriesId">ID of a series</param>
        public static void PlayRandomEpisode(int seriesId)
        {
            List<DBEpisode> episodes = DBEpisode.Get(seriesId);
            if (episodes == null || episodes.Count == 0) return;

            // filter out anything we can't play
            episodes.RemoveAll(e => string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episodes.Count == 0) return;


            DBEpisode episode = episodes.GetRandomElement<DBEpisode>();
            if (episode != null)
            {
                PlayEpisode(episode);
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
        public static void Play(int seriesId, int seasonNumer, int episodeNumber)
        {
            var episodes = DBEpisode.Get(seriesId, seasonNumer);
            var episode = episodes.FirstOrDefault(e => (e[DBEpisode.cEpisodeIndex] == episodeNumber || e[DBEpisode.cEpisodeIndex2] == episodeNumber) && !string.IsNullOrEmpty(e[DBEpisode.cFilename]));
            if (episode == null) return;

            PlayEpisode(episode);
        }

        /// <summary>
        /// Play an episode
        /// 
        /// Thanks to Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/TVSeries.cs
        /// </summary>
        /// <param name="episode">A valid tvseries episode</param>
        /// <returns></returns>
        private static void PlayEpisode(DBEpisode episode)
        {
            if (player == null) player = new VideoHandler();
            player.ResumeOrPlay(episode);
        }
    }
}
