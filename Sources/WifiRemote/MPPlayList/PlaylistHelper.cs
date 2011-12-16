using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Playlists;
using WifiRemote.MPPlayList;
using MediaPortal.GUI.Library;
using MediaPortal.Music.Database;
using MediaPortal.Video.Database;
using MediaPortal.Util;
using MediaPortal.Configuration;
using System.IO;

namespace WifiRemote.MPPlayList
{
    class PlaylistHelper
    {
        protected delegate void StartPlayingPlaylistDelegate(bool switchToPlaylistView);
        private static int mPlaylistStartIndex = 0;
        private static PlayListType mPlaylistStartType;

                /// <summary>
        /// Adds a song to a playlist
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="file">File that gets added</param>
        /// <param name="index">Index where the item should be added</param>
        public static void AddSongToPlaylist(String type, String file, int index)
        {
            PlaylistEntry entry = new PlaylistEntry();
            FileInfo fileInfo = new FileInfo(file);
            entry.FileName = fileInfo.FullName;
            entry.Name = fileInfo.Name;

            AddSongToPlaylist(type, entry, index);
        }

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
                    item = ToPlayListItem(song);
                }
            }
            else if (plType == PlayListType.PLAYLIST_VIDEO)
            {
                IMDBMovie movie = new IMDBMovie();
                int id = VideoDatabase.GetMovieInfo(entry.FileName, ref movie);

                if (id > 0)
                {
                    item = ToPlayListItem(movie);
                }
            }
            
            if(item == null){
                item = new PlayListItem(entry.Name, entry.FileName, entry.Duration);
            }

            playList.Insert(item, index);
        }

        /// <summary>
        /// Adds songs to a playlist by querying the music database
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="where">SQL where condition</param>
        /// <param name="limit">Maximum number of songs</param>
        /// <param name="shuffle"><code>true</code> to shuffle the playlist</param>
        /// <param name="startIndex">Index to at the songs at</param>
        public static void AddSongsToPlaylistWithSQL(string type, string where, int limit, bool shuffle, int startIndex)
        {
            // Only works for music atm
            PlayListType plType = GetTypeFromString(type);
            if (plType == PlayListType.PLAYLIST_MUSIC)
            {
                List<Song> songs = new List<Song>();

                string sql = "select * from tracks where " + where;
                if (shuffle)
                {
                    sql += " ORDER BY random()";
                }

                MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");
                if (songs.Count > 0)
                {
                    PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
                    int numberOfSongsAvailable = songs.Count - 1;

                    // Limit 0 means unlimited
                    if (limit == 0) limit = songs.Count;

                    for (int i = 0; i < limit && i < songs.Count; i++)
                    {
                        PlayListItem playListItem = ToPlayListItem(songs[i]);
                        playListPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC).Insert(playListItem, startIndex + i);
                    }
                }
            }
        }

        /// <summary>
        /// Load a playlist from disc.
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        /// <param name="name">Name of the playlist (file)</param>
        /// <param name="shuffle"><code>true</code> to shuffle the playlist</param>
        public static void LoadPlaylist(string type, string name, bool shuffle)
        {
            // Only working for music atm
            PlayListType plType = GetTypeFromString(type);
            
            if (plType == PlayListType.PLAYLIST_MUSIC)
            {
                string playlistPath = String.Empty;

                // Playlist path supplied
                if (name.EndsWith(".m3u"))
                {
                    playlistPath = name;
                }
                // Playlist name supplied
                else
                {
                    // Get playlist folder from mp config
                    using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                    {
                        string playlistFolder = reader.GetValueAsString("music", "playlists", "");

                        if (!Path.IsPathRooted(playlistFolder))
                        {
                            playlistFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), playlistFolder);
                        }

                        playlistPath = Path.Combine(playlistFolder, name + ".m3u");
                    }
                }

                if (File.Exists(playlistPath))
                {
                    // Load playlist from file
                    PlayListPlayer playListPlayer = PlayListPlayer.SingletonPlayer;
                    PlayList playList = playListPlayer.GetPlaylist(PlayListType.PLAYLIST_MUSIC);
                    PlayListM3uIO m3uPlayList = new PlayListM3uIO();
                    m3uPlayList.Load(playList, playlistPath);

                    // Shuffle playlist
                    if (shuffle)
                    {
                        Shuffle(type);
                    }
                }
            }
        }

        /// <summary>
        /// Shuffle a playlist
        /// </summary>
        /// <param name="type">Type of the playlist</param>
        public static void Shuffle(string type)
        {
            PlayListType plType = GetTypeFromString(type);
            PlayListPlayer.SingletonPlayer.GetPlaylist(plType).Shuffle();
        }
       
        /// <summary>
        /// Returns a playlistitem from a song
        /// 
        /// Note: this method is available in MediaPortal 1.2 in 
        /// MediaPortal.Music.Database.Song.ToPlayListItem()
        /// 
        /// As it wasn't available in 1.1.3 this was copied to a private method
        /// TODO: Remove this once 1.2 has become stable and 1.1.3 is not needed anymore
        /// </summary>
        /// <param name="song">Song</param>
        /// <returns>Playlistitem from a song</returns>
        private static PlayListItem ToPlayListItem(Song song)
        {
            PlayListItem pli = new PlayListItem();

            pli.Type = PlayListItem.PlayListItemType.Audio;
            pli.FileName = song.FileName;
            pli.Description = song.Title;
            pli.Duration = song.Duration;
            pli.MusicTag = song.ToMusicTag();

            return pli;
        }

        private static PlayListItem ToPlayListItem(IMDBMovie movie)
        {
            PlayListItem pli = new PlayListItem();

            pli.Type = PlayListItem.PlayListItemType.Video;
            pli.FileName = movie.File;
            pli.Description = movie.Title;
            pli.Duration = movie.RunTime;
   
            return pli;
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
                WifiRemote.LogMessage("Change playlist index " + oldIndex + " to " + newIndex, WifiRemote.LogType.Debug);
                PlayListItem item = playList[oldIndex];
                playList.Remove(item.FileName);
                //Note: we need -1 here, because Insert wants the item after which the song should be inserted, not the actual index
                playList.Insert(item, newIndex - 1);
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
        /// <param name="switchToPlaylistView"><code>true</code> to switch to playlist view</param>
        public static void StartPlayingPlaylist(String type, int index, bool switchToPlaylistView)
        {
            mPlaylistStartIndex = index;
            mPlaylistStartType = GetTypeFromString(type);

            StartPlayingPlaylist(switchToPlaylistView);
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
        private static void StartPlayingPlaylist(bool switchToPlaylistView)
        {
            if (GUIGraphicsContext.form.InvokeRequired)
            {
                StartPlayingPlaylistDelegate d = StartPlayingPlaylist;
                GUIGraphicsContext.form.Invoke(d, new object[] { switchToPlaylistView });
            }
            else
            {
                PlayListPlayer playlistPlayer = PlayListPlayer.SingletonPlayer;
                PlayList playlist = playlistPlayer.GetPlaylist(mPlaylistStartType);
                // if we got a playlist
                if (playlist.Count > 0)
                {
                    // and activate the playlist window if its not activated yet
                    if (switchToPlaylistView)
                    {
                        if (mPlaylistStartType == PlayListType.PLAYLIST_MUSIC)
                        {
                            GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_MUSIC_PLAYLIST);
                        }
                        else if (mPlaylistStartType == PlayListType.PLAYLIST_VIDEO)
                        {
                            GUIWindowManager.ActivateWindow((int)MediaPortal.GUI.Library.GUIWindow.Window.WINDOW_VIDEO_PLAYLIST);
                        }
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
