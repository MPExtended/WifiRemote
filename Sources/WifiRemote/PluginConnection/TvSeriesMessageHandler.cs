using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Handler for MPTvSeries messages
    /// </summary>
    internal class TvSeriesMessageHandler
    {
        /// <summary>
        /// Handle an MPTvSeries message received from a client
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="socketServer">Socket server</param>
        /// <param name="sender">Sender</param>
        internal static void HandleTvSeriesMessage(Newtonsoft.Json.Linq.JObject message, SocketServer socketServer, Deusty.Net.AsyncSocket sender)
        {
            string action = (string)message["Action"];

            if (!string.IsNullOrEmpty(action))
            {
                // A series id is needed for the following actions. 
                // If a series id was supplied we use this, otherwise we
                // try to get an id by the series name.
                //
                // If this isn't successful we do nothing.
                int? seriesId = (int?)message["SeriesId"];
                string seriesName = (string)message["SeriesName"];
                bool resume = (message["AskToResume"] != null) ? (bool)message["AskToResume"] : true;

                // Get series id by show name if no id supplied
                if (seriesId == null && !string.IsNullOrEmpty(seriesName))
                {
                    seriesId = TVSeriesHelper.GetSeriesIdByName(seriesName);
                }

                if (seriesId != null)
                {
                    // Play specific episode of series
                    if (action == "playepisode")
                    {
                        int? season = (int?)message["SeasonNumber"];
                        int? episode = (int?)message["EpisodeNumber"];
                        int startPos = (message["StartPosition"] != null) ? (int)message["StartPosition"] : 0;

                        if (season != null && episode != null)
                        {
                            TVSeriesHelper.Play((int)seriesId, (int)season, (int)episode, resume, startPos);
                        }
                    }
                    // Show movie details for this movie
                    else if (action == "seriesdetails")
                    {
                        TVSeriesHelper.ShowSeriesDetails((int)seriesId);
                    }
                    // Play first unwatched or last added episode of a series
                    else if (action == "playunwatchedepisode")
                    {
                        TVSeriesHelper.PlayFirstUnwatchedEpisode((int)seriesId, resume);
                    }
                    // Play random episode of a series
                    else if (action == "playrandomepisode")
                    {
                        TVSeriesHelper.PlayRandomEpisode((int)seriesId, resume);
                    }
                    // Play all episodes of a season
                    else if (action == "playseason")
                    {
                        int? season = (int?)message["SeasonNumber"];
                        bool onlyUnwatched = (message["OnlyUnwatchedEpisodes"] != null) ? (bool)message["OnlyUnwatchedEpisodes"] : false;
                        int startIndex = (message["StartIndex"] != null) ? (int)message["StartIndex"] : 0;
                        bool switchToPlaylistView = (message["SwitchToPlaylist"] != null) ? (bool)message["SwitchToPlaylist"] : true;
                        bool startAutomatically = (message["AutoStart"] != null) ? (bool)message["AutoStart"] : true;
                        if (season != null)
                        {
                            TVSeriesHelper.PlaySeason((int)seriesId, (int)season, startAutomatically, startIndex, onlyUnwatched, switchToPlaylistView);
                        }
                    }
                    // Play all episodes of a series
                    else if (action == "playseries")
                    {
                        bool onlyUnwatched = (message["OnlyUnwatchedEpisodes"] != null) ? (bool)message["OnlyUnwatchedEpisodes"] : false;
                        int startIndex = (message["StartIndex"] != null) ? (int)message["StartIndex"] : 0;
                        bool switchToPlaylistView = (message["SwitchToPlaylist"] != null) ? (bool)message["SwitchToPlaylist"] : true;
                        bool startAutomatically = (message["AutoStart"] != null) ? (bool)message["AutoStart"] : true;

                        TVSeriesHelper.PlaySeries((int)seriesId, startAutomatically, startIndex, onlyUnwatched, switchToPlaylistView);
                    }
                }
            }
        }
    }
}
