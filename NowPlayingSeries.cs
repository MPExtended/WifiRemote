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

        string episode;
        public string Episode
        {
            get { return episode; }
            set { episode = value; }
        }

        string season;
        public string Season
        {
            get { return season; }
            set { season = value; }
        }

        string plot;
        public string Plot
        {
            get { return plot; }
            set { plot = value; }
        }

        string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        string rating;
        public string Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        string airDate;
        public string AirDate
        {
            get { return airDate; }
            set { airDate = value; }
        }

        byte[] image;
        public byte[] Image
        {
            get { return image; }
            set { image = value; }
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

                    // Episode = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeIndex];
                    Episode = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeIndex").GetValue(null) }).ToString();
                    
                    // Season = episodes[0].onlineEpisode[DBOnlineEpisode.cSeasonIndex];
                    Season = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cSeasonIndex").GetValue(null) }).ToString();

                    // Plot = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeSummary];
                    Plot = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeSummary").GetValue(null) }).ToString();

                    // Title = episodes[0].onlineEpisode[DBOnlineEpisode.cEpisodeName];
                    Title = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cEpisodeName").GetValue(null) }).ToString();

                    // Rating = episodes[0].onlineEpisode[DBOnlineEpisode.cRating];
                    Rating = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cRating").GetValue(null) }).ToString();

                    // AirDate = episodes[0].onlineEpisode[DBOnlineEpisode.cFirstAired];
                    AirDate = item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cFirstAired").GetValue(null) }).ToString();

                    // Get season poster as thumb image
                    // DBSeason season = DBSeason.getRaw(seriesID, index);
                    // string posterFileName = WindowPlugins.GUITVSeries.ImageAllocator.GetSeasonBannerAsFilename(season);
                    Type dbSeasonType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.DBSeason");
                    Type imageAllocatorType = MPTVSeries.GetType("WindowPlugins.GUITVSeries.ImageAllocator");

                    int seriesId = Int32.Parse(item.GetValue(onlineEpisode, new object[] { onlineEpisodeType.GetField("cSeriesID").GetValue(null) }).ToString());
                    int seasonId = Int32.Parse(Season);
                    object season = dbSeasonType.InvokeMember("getRaw",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                        null,
                        null,
                        new object[] { seriesId, seasonId });

                    string imageFilename = imageAllocatorType.InvokeMember("GetSeasonBannerAsFilename",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                        null,
                        null,
                        new object[] { season }).ToString();

                    if (File.Exists(imageFilename))
                    {
                        Image fullsizeImage = Bitmap.FromFile(imageFilename);
                        int newWidth = 480;
                        int maxHeight = 640;

                        // Prevent using images internal thumbnail
                        fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                        if (fullsizeImage.Width <= newWidth)
                        {
                            newWidth = fullsizeImage.Width;
                        }

                        int NewHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
                        if (NewHeight > maxHeight)
                        {
                            // Resize with height instead
                            newWidth = fullsizeImage.Width * maxHeight / fullsizeImage.Height;
                            NewHeight = maxHeight;
                        }

                        Image newImage = fullsizeImage.GetThumbnailImage(newWidth, NewHeight, null, IntPtr.Zero);

                        // Clear handle to original file so that we can overwrite it if necessary
                        fullsizeImage.Dispose();

                        Image = WifiRemote.imageToByteArray(newImage);
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
