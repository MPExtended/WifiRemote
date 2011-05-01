using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Video.Database;
using System.IO;
using System.Drawing;

namespace WifiRemote
{
    class NowPlayingVideo : IAdditionalNowPlayingInfo
    {
        string mediaType = "video";
        public string MediaType
        {
            get { return mediaType; }
        }

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
        
        string imageUrl;
        /// <summary>
        /// Movie poster
        /// </summary>
        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aMovie">The currently playing movie</param>
        public NowPlayingVideo(IMDBMovie aMovie)
        {
            Summary = aMovie.Plot;
            Title = aMovie.Title;
            Tagline = aMovie.TagLine;
            Directors = aMovie.Director;
            Writers = aMovie.WritingCredits;
            Actors = aMovie.Cast;
            Rating = aMovie.Rating.ToString();
            Year = aMovie.Year;
            Genres = aMovie.Genre;
            Certification = aMovie.MPARating;

            ImageUrl = aMovie.ThumbURL;
        }
    }
}
