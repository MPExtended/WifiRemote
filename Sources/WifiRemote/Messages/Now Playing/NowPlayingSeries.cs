using System;
using System.Collections.Generic;
using WindowPlugins.GUITVSeries;
using WifiRemote.MpExtended;


namespace WifiRemote
{
    class NowPlayingSeries : IAdditionalNowPlayingInfo
    {
        bool episodeFound = false;

        string mediaType = "series";
        public string MediaType
        {
            get { return mediaType; }
        }

        public string MpExtId
        {
            get { return EpisodeId.ToString(); }
        }

        public int MpExtMediaType
        {
            get { return (int)MpExtendedMediaTypes.TVEpisode; }
        }

        public int MpExtProviderId
        {
            get { return (int)MpExtendedProviders.MPTvSeries; }
        }

        int seriesId;
        /// <summary>
        /// ID of the series in TVSeries' DB
        /// </summary>
        public int SeriesId
        {
            get { return seriesId; }
            set { seriesId = value; }
        }

        int seasonId;
        /// <summary>
        /// ID of the season in TVSeries' DB
        /// </summary>
        public int SeasonId
        {
            get { return seasonId; }
            set { seasonId = value; }
        }

        int episodeId;
        /// <summary>
        /// ID of the episode in TVSeries' DB
        /// </summary>
        public int EpisodeId
        {
            get { return episodeId; }
            set { episodeId = value; }
        }

        string series;
        /// <summary>
        /// Series name
        /// </summary>
        public string Series
        {
            get { return series; }
            set { series = value; }
        }

        string episode;
        /// <summary>
        /// Episode number
        /// </summary>
        public string Episode
        {
            get { return episode; }
            set { episode = value; }
        }

        string season;
        /// <summary>
        /// Season number
        /// </summary>
        public string Season
        {
            get { return season; }
            set { season = value; }
        }

        string plot;
        /// <summary>
        /// Plot summary
        /// </summary>
        public string Plot
        {
            get { return plot; }
            set { plot = value; }
        }

        string title;
        /// <summary>
        /// Episode title
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        string director;
        /// <summary>
        /// Director of this episode
        /// </summary>
        public string Director 
        { 
            get { return director; } 
            set { director = value; } 
        }

        string writer;
        /// <summary>
        /// Writer of this episode
        /// </summary>
        public string Writer
        {
            get { return writer; }
            set { writer = value; }
        }

        string rating;
        /// <summary>
        /// Online episode rating
        /// </summary>
        public string Rating
        {
            get { return rating; }
            set 
            {
                // Shorten to 3 chars, ie
                // 5.67676767 to 5.6
                if (value.Length > 3)
                {
                    value = value.Remove(3);
                }
                rating = value; 
            }
        }

        string myRating;
        /// <summary>
        /// My episode rating
        /// </summary>
        public string MyRating
        {
            get { return myRating; }
            set { myRating = value; }
        }

        string ratingCount;
        /// <summary>
        /// Number of online votes
        /// </summary>
        public string RatingCount 
        { 
            get { return ratingCount; }
            set { ratingCount = value; }
        }

        string airDate;
        /// <summary>
        /// Episode air date
        /// </summary>
        public string AirDate
        {
            get { return airDate; }
            set { airDate = value; }
        }

        string status;
        /// <summary>
        /// Status of the series
        /// </summary>
        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        string genre;
        /// <summary>
        /// Genre of the series
        /// </summary>
        public string Genre
        {
            get { return genre; }
            set { genre = value; }
        }

        string imageName;
        /// <summary>
        /// Season poster filepath
        /// </summary>
        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }




        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename">Filename of the currently played episode</param>
        public NowPlayingSeries(string filename)
        {
            try
            {
                SQLCondition query = new SQLCondition(new DBEpisode(), DBEpisode.cFilename, filename, SQLConditionType.Equal);
                List<DBEpisode> episodes = DBEpisode.Get(query);

                if (episodes.Count > 0)
                {
                    episodeFound = true;
                    
                    SeriesId = episodes[0].onlineEpisode[DBOnlineEpisode.cSeriesID];
                    SeasonId = episodes[0].onlineEpisode[DBOnlineEpisode.cSeasonID];
                    EpisodeId = episodes[0].onlineEpisode[DBOnlineEpisode.cID];

                    Episode = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
                    Season = episodes[0].onlineEpisode[DBOnlineEpisode.cSeasonIndex];
                    Plot = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeSummary];
                    Title = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeName];
                    Director = episodes[0].onlineEpisode[DBOnlineEpisode.cDirector];
                    Writer = episodes[0].onlineEpisode[DBOnlineEpisode.cWriter];
                    Rating = episodes[0].onlineEpisode[DBOnlineEpisode.cRating];
                    MyRating = episodes[0].onlineEpisode[DBOnlineEpisode.cMyRating];
                    RatingCount = episodes[0].onlineEpisode[DBOnlineEpisode.cRatingCount];
                    AirDate = episodes[0].onlineEpisode[DBOnlineEpisode.cFirstAired];

                    DBSeries s = Helper.getCorrespondingSeries(episodes[0].onlineEpisode[DBOnlineEpisode.cSeriesID]);
                    Series = s[DBOnlineSeries.cPrettyName];
                    Status = s[DBOnlineSeries.cStatus];
                    Genre = s[DBOnlineSeries.cGenre];

                    // Get season poster path
                    DBSeason season = DBSeason.getRaw(SeriesId, episodes[0].onlineEpisode[DBOnlineEpisode.cSeasonIndex]);
                    ImageName = ImageAllocator.GetSeasonBannerAsFilename(season);

                    // Fall back to series poster if no season poster is available
                    if (String.IsNullOrEmpty(ImageName))
                    {
                        ImageName = ImageAllocator.GetSeriesPosterAsFilename(s);
                    }
                }            
            }
            catch (Exception e)
            {
                WifiRemote.LogMessage("Error getting now playing tvseries: " + e.Message, WifiRemote.LogType.Error);
            }
        }

        /// <summary>
        /// Is this file a tv series episode?
        /// </summary>
        /// <returns><code>true</code> if the file is a tv series episode</returns>
        public bool IsEpisode()
        {
            return episodeFound;
        }
    }
}
