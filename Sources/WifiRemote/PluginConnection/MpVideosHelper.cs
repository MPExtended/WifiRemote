using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Video.Database;
using System.Collections;
using MediaPortal.GUI.Video;
using System.IO;
using WifiRemote.MPPlayList;

namespace WifiRemote.PluginConnection
{
    class MpVideosHelper
    {
        /// <summary>
        /// Plays a MpVideos movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startPos"></param>
        internal static void PlayVideo(int id, int startPos)
        {
            //Code mostly copied from WindowPlugins -> GUIVideoFiles -> GuiVideoTitle.cs
            IMDBMovie movie = new IMDBMovie();
            VideoDatabase.GetMovieInfoById(id, ref movie);
            if (movie == null && movie.ID < 0)
            {
                WifiRemote.LogMessage("No video found for id " + id, WifiRemote.LogType.Warn);
                return;
            }
            GUIVideoFiles.Reset(); // reset pincode

            ArrayList files = new ArrayList();
            VideoDatabase.GetFiles(movie.ID, ref files);

            if (files.Count > 1)
            {
                GUIVideoFiles._stackedMovieFiles = files;
                GUIVideoFiles._isStacked = true;
                GUIVideoFiles.MovieDuration(files);
            }
            else
            {
                GUIVideoFiles._isStacked = false;
            }
            GUIVideoFiles.PlayMovie(movie.ID, false);
        }

        internal static void PlayFolder(String folder, string[] extensions, int startPos)
        {
            WifiRemote.LogMessage("Adding all files in " + folder + " to current playlist", WifiRemote.LogType.Debug);
            if (Directory.Exists(folder))
            {
                PlaylistHelper.ClearPlaylist("video");

                int index = 0;
                foreach (String f in Directory.GetFiles(folder))
                {
                    if (isValidExtension(f, extensions))
                    {
                        PlaylistHelper.AddSongToPlaylist("video", f, index);
                        index++;
                    }
                }

                PlaylistHelper.StartPlayingPlaylist("video", startPos, true);
            }
            else
            {
                WifiRemote.LogMessage("Folder " + folder + " doesn't exist", WifiRemote.LogType.Warn);
            }
        }

        /// <summary>
        /// Checks if filename has a valid extension
        /// </summary>
        /// <param name="filename">Filename to check</param>
        /// <param name="extensions">Valid extensions</param>
        /// <returns></returns>
        private static bool isValidExtension(string filename, string[] extensions)
        {
            foreach (string e in extensions)
            {
                if (filename.EndsWith(e, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        internal static void PlayVideoFromFile(String file, int startPos)
        {
            if (File.Exists(file))
            {
                new Communication().PlayVideoFile(file, startPos, "video");
            }
            else
            {
                WifiRemote.LogMessage("File " + file + " doesn't exist", WifiRemote.LogType.Warn);
            }
        }
    }
}
