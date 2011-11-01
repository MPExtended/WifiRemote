using System;
using MediaPortal.Plugins.MovingPictures.Database;
using WifiRemote.MpExtended;
using MediaPortal.GUI.Library;

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

        public string MpExtId
        {
            get { return ItemId.ToString(); }
        }

        public int MpExtMediaType
        {
            get { return (int)MpExtendedMediaTypes.Movie; }
        }

        public int MpExtProviderId
        {
            get { return (int)MpExtendedProviders.MovingPictures; }
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
                DBLocalMedia possibleMatches = DBLocalMedia.Get(filename);
                if (possibleMatches.AttachedMovies.Count > 0)
                {
                    movieFound = true;
                    DBMovieInfo match = possibleMatches.AttachedMovies[0];

                    ItemId = (int)match.ID;
                    Title = match.Title;
                    AlternateTitles = match.AlternateTitles.ToString();
                    Directors = match.Directors.ToString();
                    Writers = match.Writers.ToString();
                    Actors = match.Actors.ToString();
                    Year = match.Year;
                    Genres = match.Genres.ToString();
                    Certification = match.Certification;
                    Tagline = match.Tagline;
                    Summary = match.Summary;
                    Rating = match.Score.ToString();
                    DetailsUrl = match.DetailsURL;
                    ImageName = match.CoverFullPath;
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
