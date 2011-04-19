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
        
        byte[] image;
        /// <summary>
        /// Movie poster
        /// </summary>
        public byte[] Image
        {
            get { return image; }
            set { image = value; }
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

            string coverPath = aMovie.ThumbURL;
            if (File.Exists(coverPath))
            {
                Image fullsizeImage = Bitmap.FromFile(coverPath);
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

                Image = WifiRemote.imageToByteArray(newImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
