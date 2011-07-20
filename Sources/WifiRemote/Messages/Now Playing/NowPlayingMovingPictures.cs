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
    class NowPlayingMovingPictures : IAdditionalNowPlayingInfo
    {
        bool movieFound = false;

        string mediaType = "movie";
        public string MediaType
        {
            get { return mediaType; }
        }

        /// <summary>
        /// Movie ID in moving pictures database table "movie_info"
        /// </summary>
        public int ItemId { get; set; }

        string summary;
        /// <summary>
        /// Plot summary
        /// </summary>
        public string Summary
        {
            get { return summary; }
            set { summary = value; }
        }

        string title;
        /// <summary>
        /// Movie title
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        string alternateTitles;
        /// <summary>
        /// Alternate titles of this movie
        /// </summary>
        public string AlternateTitles
        {
            get { return alternateTitles; }
            set { alternateTitles = value; }
        }

        string tagline;
        /// <summary>
        /// Tagline of the movie
        /// </summary>
        public string Tagline
        {
            get { return tagline; }
            set { tagline = value; }
        }

        string directors;
        /// <summary>
        /// Director of this movie
        /// </summary>
        public string Directors
        {
            get { return directors; }
            set { directors = value; }
        }

        string writers;
        /// <summary>
        /// Writer of this movie
        /// </summary>
        public string Writers
        {
            get { return writers; }
            set { writers = value; }
        }

        string actors;
        /// <summary>
        /// Actors in this movie
        /// </summary>
        public string Actors
        {
            get { return actors; }
            set { actors = value; }
        }



        string rating;
        /// <summary>
        /// Online rating
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

        int year;
        /// <summary>
        /// Movie air date
        /// </summary>
        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        string genres;
        /// <summary>
        /// Genres of the movie
        /// </summary>
        public string Genres
        {
            get { return genres; }
            set { genres = value; }
        }

        string certification;
        /// <summary>
        /// Certification of the movie
        /// </summary>
        public string Certification
        {
            get { return certification; }
            set { certification = value; }
        }

        string detailsUrl;
        /// <summary>
        /// Get more info about the movie at this URL
        /// </summary>
        public string DetailsUrl
        {
            get { return detailsUrl; }
            set { detailsUrl = value; }
        }

        string imageName;
        /// <summary>
        /// Movie poster filepath
        /// </summary>
        public string ImageName
        {
            get { return imageName; }
            set { imageName = value; }
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filename">Filename of the currently played media file</param>
        public NowPlayingMovingPictures(string filename)
        {
            try
            {
                // DBLocalMedia.Get("c:\my\video.avi").AttachedMovies[0];
                Assembly MovingPictures = Assembly.Load("MovingPictures");
                Type dbLocalMediaType = MovingPictures.GetType("MediaPortal.Plugins.MovingPictures.Database.DBLocalMedia");

                // MethodInfo: MediaPortal.Plugins.MovingPictures.Database.DBLocalMedia Get(System.String)
                object movie = dbLocalMediaType.InvokeMember("Get",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
                        null,
                        null,
                        new object[] { filename });

                // PropertyInfo: Cornerstone.Database.CustomTypes.RelationList`2[MediaPortal.Plugins.MovingPictures.Database.DBLocalMedia,MediaPortal.Plugins.MovingPictures.Database.DBMovieInfo] AttachedMovies
                PropertyInfo attachedMoviesProp = dbLocalMediaType.GetProperty("AttachedMovies");
                IList attachedMovies = (IList)attachedMoviesProp.GetValue(movie, null);

                if (attachedMovies.Count > 0)
                {
                    movieFound = true;

                    Type dbMovieInfoType = MovingPictures.GetType("MediaPortal.Plugins.MovingPictures.Database.DBMovieInfo");
                    PropertyInfo[] movieProperties = dbMovieInfoType.GetProperties();
                    foreach (PropertyInfo movieProp in movieProperties)
                    {
                        switch (movieProp.ToString())
                        {
                            case "System.Nullable`1[System.Int32] ID":
                                ItemId = (int)movieProp.GetValue(attachedMovies[0], null);
                                break;

                            case "System.String Title":
                                Title = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Cornerstone.Database.CustomTypes.StringList AlternateTitles":
                                AlternateTitles = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Cornerstone.Database.CustomTypes.StringList Directors":
                                Directors = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Cornerstone.Database.CustomTypes.StringList Writers":
                                Writers = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Cornerstone.Database.CustomTypes.StringList Actors":
                                Actors = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Int32 Year":
                                Year = (int)movieProp.GetValue(attachedMovies[0], null);
                                break;

                            case "Cornerstone.Database.CustomTypes.StringList Genres":
                                Genres = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "System.String Certification":
                                Certification = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "System.String Tagline":
                                Tagline = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "System.String Summary":
                                Summary = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "Single Score":
                                Rating = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "System.String DetailsURL":
                                DetailsUrl = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;

                            case "System.String CoverFullPath":
                                ImageName = movieProp.GetValue(attachedMovies[0], null).ToString();
                                break;
                        }
                    }
                
                }
            }
            catch (Exception e)
            {
                WifiRemote.LogMessage("Error getting now playing moving pictures: " + e.Message, WifiRemote.LogType.Error);
            }
        }

        /// <summary>
        /// Checks if the supplied filename is a moving pictures movie
        /// </summary>
        /// <returns></returns>
        public bool IsMovingPicturesMovie()
        {
            return movieFound;
        }
    }
}
