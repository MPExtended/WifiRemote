using System.Collections.Generic;
using Cornerstone.Database;
using MediaPortal.Plugins.MovingPictures.Database;
using MediaPortal.Plugins.MovingPictures;
using MediaPortal.Plugins.MovingPictures.MainUI;
using MediaPortal.GUI.Library;
using System.Threading;
using MediaPortal.Player;

namespace WifiRemote
{
    class MovingPicturesHelper
    {
        public static MoviePlayer player = null;
        protected delegate void PlayMovieAsyncDelegate(DBMovieInfo movie, bool resume, int startPosition);

        /// <summary>
        /// Get a MovingPictures movie id that matches a movie
        /// for the supplied file name
        /// </summary>
        /// <param name="movieName">A movie name</param>
        /// <returns>A movie id or -1 if no movie was found</returns>
        public static int GetMovieIdByName(string movieName)
        {
            DBMovieInfo movie = GetMovieByName(movieName);
            return (movie != null && movie.ID != null) ? (int)movie.ID : -1;
        }

        /// <summary>
        /// Get a movie object by movie name
        /// </summary>
        /// <param name="movieName">Name of a movie</param>
        /// <returns>Returns a movie object or null if no movie was found.</returns>
        private static DBMovieInfo GetMovieByName(string movieName)
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
                        return movie;
                    }
                }

                return foundMovies[0];
            }
            else
            {
                // Return the first and only movie or null if there was no result
                if (foundMovies.Count == 1)
                {
                    return foundMovies[0];
                }
                else
                {
                    WifiRemote.LogMessage("Could not find MovingPictures movie " + movieName, WifiRemote.LogType.Info);
                    return null;
                }
            }

        }

        /// <summary>
        /// Play a movie with MovingPictures by name.
        /// </summary>
        /// <param name="movieName">Name of the movie to play</param>
        /// <param name="resume">Ask to resume movie?</param>
        public static void PlayMovie(string movieName, bool resume, int startPosition = 0)
        {
            PlayMovie(GetMovieByName(movieName), resume, startPosition);
        }

        /// <summary>
        /// Play a movie with MovingPictures by ID.
        /// </summary>
        /// <param name="movieId">A MovingPictures movie id.</param>
        /// <param name="resume">Ask to resume movie?</param>
        public static void PlayMovie(int movieId, bool resume, int startPosition = 0)
        {
            DBMovieInfo movie = DBMovieInfo.Get(movieId);
            if (movie == null)
            {
                WifiRemote.LogMessage("Could not find MovingPictures movie with id " + movieId.ToString(), WifiRemote.LogType.Info);
            }
            else
            {
                PlayMovie(movie, resume, startPosition);
            }
        }

        /// <summary>
        /// Play a movie with MovingPictures.
        ///
        /// Taken from Trakt-for-MediaPortal:
        /// https://github.com/Technicolour/Trakt-for-Mediaportal/blob/master/TraktPlugin/TraktHandlers/MovingPictures.cs
        /// </summary>
        /// <param name="movie">Movie to play</param>
        /// <param name="resume">Ask to resume movie?</param>
        public static void PlayMovie(DBMovieInfo movie, bool resume, int startPosition = 0)
        {
            if (movie == null) return;

            // Play on a new thread
            ThreadStart ts = delegate() { DoPlayMovie(movie, resume, startPosition); };
            Thread playMovieAsync = new Thread(ts);
            playMovieAsync.Start();
        }

        /// <summary>
        /// Start playing a movie on a seperate thread
        /// </summary>
        /// <param name="movie">Movie to play</param>
        /// <param name="resume">Ask user to resume?</param>
        private static void DoPlayMovie(DBMovieInfo movie, bool resume, int startPosition)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                PlayMovieAsyncDelegate d = new PlayMovieAsyncDelegate(DoPlayMovie);
                GUIGraphicsContext.form.Invoke(d, new object[] { movie, resume, startPosition });
                return;
            }
            WifiRemote.LogMessage("Play movie (resume: " + resume + ", pos: " + startPosition, WifiRemote.LogType.Debug);

            // Clear resume
            if (!resume && movie.UserSettings != null && movie.UserSettings.Count > 0)
            {
                DBUserMovieSettings userSetting = movie.ActiveUserSettings;
                userSetting.ResumePart = 0;
                userSetting.ResumeTime = 0;
                userSetting.ResumeData = null;
                userSetting.Commit();
            }

            if (player == null) player = new MoviePlayer(new MovingPicturesGUI());
            player.Play(movie);

            if (!resume && startPosition > 0)
            {
                g_Player.SeekAbsolute(startPosition);
            }
        }
    }
}
