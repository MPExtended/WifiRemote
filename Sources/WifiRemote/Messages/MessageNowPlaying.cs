using System;
using MediaPortal.Player;
using MediaPortal.Video.Database;
using MediaPortal.GUI.Library;
using MediaPortal.Music.Database;
using WifiRemote.PluginConnection;

namespace WifiRemote
{
    class MessageNowPlaying : IMessage
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

        public String Type
        {
            get { return "nowplaying"; }
        }

        /// <summary>
        /// Duration of the media in seconds
        /// </summary>
        public int Duration
        {
            get 
            {
                if (!g_Player.Playing)
                {
                    return 0;
                }

                return (int)g_Player.Player.Duration; 
            }
        }

        /// <summary>
        /// The filename of the currently playing item
        /// </summary>
        public String File
        {
            get 
            {
                if (!g_Player.Playing)
                {
                    return String.Empty;
                }

                return g_Player.CurrentFile; 
            }
        }

        /// <summary>
        /// Current position in the file in seconds
        /// </summary>
        public int Position
        {
            get 
            {
                if (!g_Player.Playing)
                {
                    return 0;
                }

                return (int)g_Player.Player.CurrentPosition; 
            }
        }

        /// <summary>
        /// Is the current playing item tv
        /// </summary>
        public bool IsTv
        {
            get 
            {
                if (!g_Player.Playing)
                {
                    return false;
                }

                return g_Player.Player.IsTV; 
            }
        }

        /// <summary>
        /// Is the player in fullscreen mode
        /// </summary>
        public bool IsFullscreen
        {
            get
            {
                return (g_Player.Playing && g_Player.FullScreen);
            }
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
                        Song song = new Song();
                        MusicDatabase musicDatabase = MusicDatabase.Instance;
                        musicDatabase.GetSongByFileName(g_Player.Player.CurrentFile, ref song);

                        // MyMusic song
                        if (song != null)
                        {
                            return new NowPlayingMusic(song);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    // Radio
                    if (g_Player.IsRadio)
                    {
                        if (!WifiRemote.IsAvailableTVPlugin)
                        {
                            WifiRemote.LogMessage("No TVPlugin installed: can't add now playing", WifiRemote.LogType.Error);
                        }
                        else
                        {
                            WifiRemote.LogMessage("Retrieve NP Radio", WifiRemote.LogType.Debug);
                            return MpTvServerHelper.GetNowPlayingRadio();                       
                        }
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
                    else if (g_Player.IsTV && WifiRemote.IsAvailableTVPlugin)
                    {
                        if (g_Player.IsTVRecording)
                        {
                            NowPlayingRecording recording = MpTvServerHelper.GetNowPlayingRecording();

                            if (recording.IsRecording())
                            {
                                return recording;
                            }
                        }
                        else
                        {
                            return MpTvServerHelper.GetNowPlayingTv();
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
            isMovingPicturesAvailable = WifiRemote.IsAssemblyAvailable("MovingPictures", new Version(1, 0, 6, 1116));
            isTVSeriesAvailable = WifiRemote.IsAssemblyAvailable("MP-TVSeries", new Version(2, 6, 3, 1242));
            isFanartHandlerAvailable = WifiRemote.IsAssemblyAvailable("FanartHandler", new Version(2, 2, 1, 19191));
        }


    }
}
