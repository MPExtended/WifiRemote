using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;
using MediaPortal.Video.Database;
using System.Reflection;

namespace WifiRemote
{
    class MessageNowPlaying
    {

        /// <summary>
        /// <code>true</code> if the moving pictures plugin is available
        /// </summary>
        private bool isMovingPicturesAvailable = false;

        /// <summary>
        /// <code>true</code> if the MP-TVSeries plugin is available
        /// </summary>
        private bool isTVSeriesAvailable = false;

        /// <summary>
        /// <code>true</code> if the FanartHandler plugin is available
        /// </summary>
        private bool isFanartHandlerAvailable = false;

        string type = "nowplaying";
        public String Type
        {
            get { return type; }
        }

        /// <summary>
        /// Duration of the media in seconds
        /// </summary>
        public int Duration
        {
            get { return (int)g_Player.Player.Duration; }
        }

        /// <summary>
        /// Current position in the file in seconds
        /// </summary>
        public int Position
        {
            get { return (int)g_Player.Player.CurrentPosition; }
        }

        public IAdditionalNowPlayingInfo MediaInfo
        {
            get 
            {
                if (g_Player.Playing)
                {
                    // Music
                    if (g_Player.IsMusic)
                    {
                        return new NowPlayingMusic();
                    }
                    // Video
                    else if (g_Player.IsVideo)
                    {
                        // DVD
                        if (MediaPortal.Util.Utils.IsDVD(g_Player.Player.CurrentFile))
                        {
                            return new NowPlayingDVD();
                        }
                        else
                        {
                            IMDBMovie movie = new IMDBMovie();
                            int movieId = VideoDatabase.GetMovieId(g_Player.Player.CurrentFile);
                            VideoDatabase.GetMovieInfoById(movieId, ref movie);

                            // MyVideos movie
                            if (movie.ID > 0)
                            {
                                return new NowPlayingVideo(movie);
                            }
                            else
                            // MovingPictures, TVSeries or something else
                            {
                                if (isTVSeriesAvailable)
                                {
                                    // Media is TVSeries episode?
                                    NowPlayingSeries series = new NowPlayingSeries(g_Player.Player.CurrentFile);
                                    if (series.IsEpisode())
                                    {
                                        return series;
                                    }
                                }
                                
                                if (isMovingPicturesAvailable)
                                {
                                    // Media is a movie managed by moving pictures?
                                    NowPlayingMovingPictures movpics = new NowPlayingMovingPictures(g_Player.Player.CurrentFile);
                                    if (movpics.IsMovingPicturesMovie())
                                    {
                                        return movpics;
                                    }
                                }

                                return null;
                            }
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Constructor. 
        /// </summary>
        public MessageNowPlaying()
        {
            // Check for "hookable" plugins
            isMovingPicturesAvailable = IsAssemblyAvailable("MovingPictures", new Version(1, 0, 6, 1116));
            isTVSeriesAvailable = IsAssemblyAvailable("MP-TVSeries", new Version(2, 6, 3, 1242));
            isFanartHandlerAvailable = IsAssemblyAvailable("FanartHandler", new Version(2, 2, 1, 19191));
        }

        /// <summary>
        /// Check if assembly is available
        /// </summary>
        /// <param name="name">Assembly name</param>
        /// <param name="ver">Assembly version</param>
        /// <returns>true if the assembly is available</returns>
        public bool IsAssemblyAvailable(string name, Version ver)
        {
            bool result = false;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {
                try
                {
                    if (a.GetName().Name == name)
                    {
                        if (a.GetName().Version >= ver)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
