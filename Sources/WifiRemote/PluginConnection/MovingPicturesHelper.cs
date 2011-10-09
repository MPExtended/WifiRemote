using System.Collections.Generic;
using Cornerstone.Database;
using MediaPortal.Plugins.MovingPictures.Database;
using MediaPortal.Plugins.MovingPictures;

namespace WifiRemote
{
    class MovingPicturesHelper
    {
        /// <summary>
        /// Get a moving pictures movie id that matches a movie
        /// for the supplied file name
        /// </summary>
        /// <param name="movieName">A movie name</param>
        /// <returns>A movie id</returns>
        public static int GetMovieIdByName(string movieName)
        {
            ICriteria titleFilter = new BaseCriteria(DBField.GetField(typeof(DBMovieInfo), "Title"), "like", "%" + movieName + "%");
            List<DBMovieInfo> foundMovies = MovingPicturesCore.DatabaseManager.Get<DBMovieInfo>(titleFilter);

            // If there are more than one result return the movie with an exact title
            // match or first movie if no exact match was found
            if (foundMovies.Count > 1)
            {
                foreach (DBMovieInfo movie in foundMovies)
                {
                    if (movie.Title.ToLower().Equals(movieName.ToLower()))
                    {
                        return (int)movie.ID;
                    }
                }

                return (int)foundMovies[0].ID;
            }
            else
            {
                // Return the first and only movie or -1 as id if there was no result
                return (foundMovies.Count == 1) ? (int)foundMovies[0].ID  : -1;
            }
        }
    }
}
