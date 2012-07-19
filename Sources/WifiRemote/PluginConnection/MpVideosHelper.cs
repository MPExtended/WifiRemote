using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Video.Database;
using System.Collections;
using MediaPortal.GUI.Video;
using System.IO;
using WifiRemote.MPPlayList;
using MediaPortal.Playlists;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Helper class for MPVideos actions
    /// </summary>
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
                    if (IsValidExtension(f, extensions))
                    {
                        PlaylistHelper.AddItemToPlaylist("video", f, index);
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
        private static bool IsValidExtension(string filename, string[] extensions)
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

        /// <summary>
        /// Play video given a file path
        /// </summary>
        /// <param name="file">File to video</param>
        /// <param name="startPos">Start position of video</param>
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

        /// <summary>
        /// Show details of a video
        /// </summary>
        /// <param name="videoId">Id of video</param>
        internal static void ShowVideoDetails(int videoId)
        {
            WifiRemote.LogMessage("Not implemented yet for mp video", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Show details of a file
        /// </summary>
        /// <param name="file">Path to file</param>
        internal static void ShowFileDetails(string file)
        {
            WifiRemote.LogMessage("Not implemented yet for mp video", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Show details of folder
        /// </summary>
        /// <param name="folder"></param>
        internal static void ShowFolderDetails(string folder)
        {
            WifiRemote.LogMessage("Not implemented yet for mp video", WifiRemote.LogType.Info);
        }

        /// <summary>
        /// Create playlist item from a file
        /// </summary>
        /// <param name="file">Path to file</param>
        /// <returns>Playlist item</returns>
        internal static MediaPortal.Playlists.PlayListItem CreatePlaylistItemFromVideoFile(string file)
        {
            FileInfo info = new FileInfo(file);
            MediaPortal.Playlists.PlayListItem item = new MediaPortal.Playlists.PlayListItem();
            item.Description = info.Name;
            item.FileName = info.FullName;
            item.Type = PlayListItem.PlayListItemType.Video;
            //item.Duration
            IMDBMovie movie = new IMDBMovie();
            int id = VideoDatabase.GetMovieInfo(file, ref movie);

            if (id > 0)
            {
                item.Duration = movie.RunTime;
            }

            return item;
        }

        /// <summary>
        /// Create playlist item from a folder
        /// </summary>
        /// <param name="folder">Path to folder</param>
        /// <returns>Playlist item</returns>
        internal static List<MediaPortal.Playlists.PlayListItem> CreatePlaylistItemFromVideoFolder(string folder, string[] extensions)
        {
            DirectoryInfo info = new DirectoryInfo(folder);
            List<MediaPortal.Playlists.PlayListItem> returnList = new List<MediaPortal.Playlists.PlayListItem>();
            FileInfo[] files = info.GetFiles();
            if (files != null)
            {
                foreach (FileInfo f in files)
                {
                    if (IsValidExtension(f.FullName, extensions))
                    {
                        returnList.Add(CreatePlaylistItemFromVideoFile(f.FullName));
                    }
                }
            }

            return returnList;
        }
    }
}
