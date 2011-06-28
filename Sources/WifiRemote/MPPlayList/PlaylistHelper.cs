using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Playlists;
using WifiRemote.MPPlayList;
using MediaPortal.GUI.Library;
using MediaPortal.Music.Database;
using MediaPortal.Video.Database;

namespace WifiRemote.MPPlayList
{
    class PlaylistHelper
    {
        protected delegate void StartPlayingPlaylistDelegate();
        private static int mPlaylistStartIndex = 0;
        private static PlayListType mPlaylistStartType;


        /// <summary>
        /// Adds a song to a playlist
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="entry">Item that gets added</param>
        /// <param name="index">Index where the item should be added</param>
        public static void AddSongToPlaylist(String type, PlaylistEntry entry, int index)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
            PlayList playList = playListPlayer.GetPlaylist(plType);
            PlayListItem item = null;

            //If it's a music item, try to find it in the db
            if (plType == PlayListType.PLAYLIST_MUSIC)
            {
                MusicDatabase mpMusicDb = MusicDatabase.Instance;
                Song song = new Song();
                bool inDb = mpMusicDb.GetSongByFileName(entry.FileName, ref song);


                if (inDb)
                {
                    item = song.ToPlayListItem();
                }
            }            
            
            if(item == null){
                item = new PlayListItem(entry.Name, entry.FileName, entry.Duration);
            }

            playList.Insert(item, index);
        }

        /// <summary>
        /// Changes the position of an item in the playlist
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="oldIndex">Current position of item</param>
        /// <param name="newIndex">Target position of item</param>
        public static void ChangePlaylistItemPosition(String type, int oldIndex, int newIndex)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
            PlayList playList = playListPlayer.GetPlaylist(plType);

            if (oldIndex >= 0 && newIndex >= 0 && playList.Count > oldIndex && playList.Count > newIndex)
            {
                PlayListItem item = playList[oldIndex];
                playList.Remove(item.FileName);
                playList.Insert(item, newIndex);
            }
            else
            {
                WifiRemote.LogMessage("Index for changing playlistPosition invalid (old: " + oldIndex + ", new: " + newIndex + ")", WifiRemote.LogType.Warn);
            }
        }

        /// <summary>
        /// Clears the playlist (removes all entries)
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        public static void ClearPlaylist(String type)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
            PlayList playList = playListPlayer.GetPlaylist(plType);
            playList.Clear();
        }

        /// <summary>
        /// Removes a song (identified by index in the playlist) from the playlist
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="index">Index that should be removed</param>
        public static void RemoveSongFromPlaylist(String type, int index)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
            PlayList playList = playListPlayer.GetPlaylist(plType);

            if (playList.Count > index)
            {
                PlayListItem item = playList[index];
                playList.Remove(item.FileName);
            }
        }

        /// <summary>
        /// Gets the playlist for a given type
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <returns>List of all playlist items</returns>
        public static List<PlaylistEntry> GetPlaylistItems(String type)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
            PlayList playList = playListPlayer.GetPlaylist(plType);

            List<PlaylistEntry> retList = new List<PlaylistEntry>();
            foreach (PlayListItem item in playList)
            {
                PlaylistEntry entry = new PlaylistEntry();
                entry.FileName = item.FileName;
                entry.Name = item.Description;
                entry.Duration = item.Duration;
                entry.Played = item.Played;
                retList.Add(entry);
            }

            return retList;
        }

        /// <summary>
        /// Starts playing the playlist from a given index
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="index">Index where playback is started</param>
        public static void StartPlayingPlaylist(String type, int index)
        {
            mPlaylistStartIndex = index;
            mPlaylistStartType = GetTypeFromString(type);

            StartPlayingPlaylist();
        }

        /// <summary>
        /// Matches the different types of playlist to their enum values
        /// from MediaPortal code.
        /// 
        /// Currently supported: music, video
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <returns>Type of the playlist</returns>
        private static PlayListType GetTypeFromString(string type)
        {
            if (type == "music")
            {
                return PlayListType.PLAYLIST_MUSIC;
            }
            else if (type == "video")
            {
                return PlayListType.PLAYLIST_VIDEO;
            }
            return PlayListType.PLAYLIST_NONE;
        }

        /// <summary>
        /// Private method to start playlist (needed for the invoke callback)
        /// </summary>
        private static void StartPlayingPlaylist()
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                StartPlayingPlaylistDelegate d = StartPlayingPlaylist;
                GUIGraphicsContext.form.Invoke(d);
            }
            else
            {
                PlayListPlayer playlistPlayer = PlayListPlayer.SingletonPlayer;
                PlayList playlist = playlistPlayer.GetPlaylist(mPlaylistStartType);
                // if we got a playlist
                if (playlist.Count > 0)
                {
                    // then get 1st song
                    PlayListItem item = playlist[0];

                    // and activate the playlist window if its not activated yet
                    if (mPlaylistStartType == PlayListType.PLAYLIST_MUSIC)
                    {
                        GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_MUSIC_PLAYLIST);
                    }
                    else if (mPlaylistStartType == PlayListType.PLAYLIST_VIDEO)
                    {
                        GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_VIDEO_PLAYLIST);
                    }

                    // and start playing it
                    playlistPlayer.CurrentPlaylistType = mPlaylistStartType;
                    playlistPlayer.Reset();
                    playlistPlayer.Play(mPlaylistStartIndex);
                }
            }
        }
    }
}
