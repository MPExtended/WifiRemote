using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Drawing;


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
                // WindowPlugins.GUITVSeries
                Assembly MPTVSeries = Assembly.Load("MP-TVSeries");
                Type sqlConditionType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.SQLCondition");
                Type sqlConditionTypeEnumType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.SQLConditionType");
                Type dbEpisodeType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBEpisode");
                Type dbValueType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBValue");
                
                // SQLCondition sql = new SQLCondition();
                object sql = Activator.CreateInstance(sqlConditionType);

                // sql.Add(new DBEpisode(), DBEpisode.cFilename, filename, SQLConditionType.Equal);
                MethodInfo addCondition = sqlConditionType.GetMethod("Add");
                addCondition.Invoke(sql, new object[] { 
                    Activator.CreateInstance(dbEpisodeType),
                    dbEpisodeType.GetField("cFilename").GetValue(null),
                    Activator.CreateInstance(dbValueType, new object[] { filename }),
                    sqlConditionTypeEnumType.GetField("Equal").GetValue(null)
                });


                // List<DBEpisode> episodes = DBEpisode.Get(sql);
                MethodInfo getEpisode = dbEpisodeType.GetMethod("Get", new Type[] { sqlConditionType });
                IList episodes = (IList)getEpisode.Invoke(null, new object[] { sql });

                if (episodes.Count > 0)
                {
                    episodeFound = true;

                    PropertyInfo onlineEpisodeProperty = dbEpisodeType.GetProperty("onlineEpisode");
                    object onlineEpisode = onlineEpisodeProperty.GetValue(episodes[0], null);

                    Type onlineEpisodeType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBOnlineEpisode");
                    PropertyInfo item = onlineEpisodeType.GetProperty("Item");

                    // TheTVDB IDs
                    SeriesId = Int32.Parse(item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cSeriesID").GetValue(null) }).ToString());
                    SeasonId = Int32.Parse(item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cSeasonID").GetValue(null) }).ToString());
                    EpisodeId = Int32.Parse(item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cID").GetValue(null) }).ToString());

                    // Episode = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
                    Episode = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeIndex").GetValue(null) }).ToString();
                    
                    // Season = episodes[0].onlineEpisode[DBOnlineEpisode.cSeasonIndex];
                    Season = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cSeasonIndex").GetValue(null) }).ToString();

                    // Plot = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeSummary];
                    Plot = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeSummary").GetValue(null) }).ToString();

                    // Title = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeName];
                    Title = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeName").GetValue(null) }).ToString();

                    // Director = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeName];
                    Director = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cDirector").GetValue(null) }).ToString();

                    // Writer = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeName];
                    Writer = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cWriter").GetValue(null) }).ToString();

                    // Rating = episodes[0].onlineEpisode[DBOnlineEpisode.cRating];
                    Rating = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cRating").GetValue(null) }).ToString();

                    // MyRating = episodes[0].onlineEpisode[DBOnlineEpisode.cMyRating];
                    MyRating = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cMyRating").GetValue(null) }).ToString();

                    // RatingCount = episodes[0].onlineEpisode[DBOnlineEpisode.cRatingCount];
                    RatingCount = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cRatingCount").GetValue(null) }).ToString();

                    // AirDate = episodes[0].onlineEpisode[DBOnlineEpisode.cFirstAired];
                    AirDate = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cFirstAired").GetValue(null) }).ToString();

                    
                    // Get series information
                    // DBSeries s = Helper.getCorrespondingSeries(episodes[0].onlineEpisode[DBOnlineEpisode.cSeriesID]);
                    // s[DBOnlineSeries.cPrettyName].ToString()
                    Type dbSeriesType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBSeries");
                    Type onlineSeriesType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBOnlineSeries");
                    
                    // Get series object
                    Type helperType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.Helper");
                    MethodInfo getCorrespondingSeries = helperType.GetMethod("getCorrespondingSeries");
                    object dbSeries = getCorrespondingSeries.Invoke(null, new object[] { SeriesId });

                    // Get pretty name
                    PropertyInfo sItem = dbSeriesType.GetProperty("Item");
                    Series = sItem.GetValue(dbSeries, new object[] { onlineSeriesType.GetField("cPrettyName").GetValue(null) }).ToString();
                    
                    // Status
                    Status = sItem.GetValue(dbSeries, new object[] { onlineSeriesType.GetField("cStatus").GetValue(null) }).ToString();

                    // Genre
                    Genre = sItem.GetValue(dbSeries, new object[] { onlineSeriesType.GetField("cGenre").GetValue(null) }).ToString();

                    // Get season poster as thumb image
                    // DBSeason season = DBSeason.getRaw(seriesID, index);
                    // string posterFileName = WindowPlugins.GUITVSeries.ImageAllocator.GetSeasonBannerAsFilename(season);
                    Type dbSeasonType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBSeason");
                    Type imageAllocatorType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.ImageAllocator");

                    int seasonId = Int32.Parse(Season);
                    object season = dbSeasonType.InvokeMember("getRaw",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                        null,
                        null,
                        new object[] { seriesId, seasonId });

                    ImageName = imageAllocatorType.InvokeMember("GetSeasonBannerAsFilename",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                        null,
                        null,
                        new object[] { season }).ToString();
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
