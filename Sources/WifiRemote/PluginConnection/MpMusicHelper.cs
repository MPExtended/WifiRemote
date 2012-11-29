using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Music.Database;
using WifiRemote.MPPlayList;
using System.IO;
using MediaPortal.Playlists;

namespace WifiRemote.PluginConnection
{
    /// <summary>
    /// Helper class for MpMusic actions
    /// </summary>
    public class MpMusicHelper
    {
        /// <summary>
        /// Plays a music track, starting from a given position
        /// </summary>
        /// <param name="trackId">Music track that is played</param>
        /// <param name="startPos">Start position within the song  (in ms)</param>
        public static void PlayMusicTrack(int trackId, int startPos)
        {
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where idTrack=" + trackId;
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                new Communication().PlayAudioFile(songs[0].FileName, startPos);
            }
        }

        /// <summary>
        /// Plays all songs from the given album (defined by artist+albumname), starting with item defined by startPos
        /// </summary>
        /// <param name="albumArtist">Artist that is played</param>
        /// <param name="album">Album that is played</param>
        /// <param name="startPos">Position from where playback is started (playlist index)</param>
        public static void PlayAlbum(String albumArtist, String album, int startPos)
        {
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where strAlbumArtist like '%" + albumArtist + "%' AND strAlbum LIKE '%" + album + "%' order by iTrack ASC";
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                PlaylistHelper.ClearPlaylist("music", false);
                List<PlayListItem> items = new List<PlayListItem>();
                foreach (Song s in songs)
                {
                    items.Add(PlaylistHelper.ToPlayListItem(s));
                }
                PlaylistHelper.AddPlaylistItems(PlayListType.PLAYLIST_MUSIC, items, 0);
                PlaylistHelper.StartPlayingPlaylist("music", startPos, true);
            }
        }

        /// <summary>
        /// Plays all songs from the given artist, starting with item defined by startPos
        /// </summary>
        /// <param name="albumArtist">Artist that is played</param>
        /// <param name="startPos">Position from where playback is started (playlist index)</param>
        internal static void PlayArtist(string albumArtist, int startPos)
        {
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where strAlbumArtist like '%" + albumArtist + "%'";
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                PlaylistHelper.ClearPlaylist("music", false);
                List<PlayListItem> items = new List<PlayListItem>();
                foreach (Song s in songs)
                {
                    items.Add(PlaylistHelper.ToPlayListItem(s));
                }
                PlaylistHelper.AddPlaylistItems(PlayListType.PLAYLIST_MUSIC, items, 0);

                PlaylistHelper.StartPlayingPlaylist("music", startPos, true);
            }
        }

        /// <summary>
        /// Plays all songs from the given folder, starting with item defined by startPos
        /// </summary>
        /// <param name="folder">Folder that is played</param>
        /// <param name="extensions">Valid extensions</param>
        /// <param name="startPos">Position from where playback is started (playlist index)</param>
        internal static void PlayAllFilesInFolder(string folder, string[] extensions, int startPos)
        {
            WifiRemote.LogMessage("Adding all files in " + folder + " to current playlist", WifiRemote.LogType.Debug);
            if (Directory.Exists(folder))
            {
                PlaylistHelper.ClearPlaylist("music", false);

                List<PlayListItem> items = new List<PlayListItem>();
                foreach (String f in Directory.GetFiles(folder))
                {
                    if (isValidExtension(f, extensions))
                    {
                        FileInfo fileInfo = new FileInfo(f);
                        items.Add(new PlayListItem(fileInfo.Name, fileInfo.FullName, 0));
                    }
                }
                PlaylistHelper.AddPlaylistItems(PlayListType.PLAYLIST_MUSIC, items, 0);

                PlaylistHelper.StartPlayingPlaylist("music", startPos, true);
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

        /// <summary>
        /// Plays a music file, starting from a given position
        /// </summary>
        /// <param name="file">File that is played</param>
        /// <param name="startPos">Start position within the song  (in ms)</param>
        internal static void PlayFile(string file, int startPos)
        {
            MusicDatabase mpMusicDb = MusicDatabase.Instance;
            Song song = new Song();
            bool inDb = mpMusicDb.GetSongByFileName(file, ref song);

            if (inDb)
            {
                //TODO: fill OSD information
                new Communication().PlayAudioFile(file, startPos);
            }
            else
            {
                new Communication().PlayAudioFile(file, startPos);
            }
        }

        internal static void ShowMusicTrackDetails(int _trackId)
        {
            WifiRemote.LogMessage("Not implemented yet for mp music", WifiRemote.LogType.Info);
        }

        internal static void ShowAlbumDetails(string p, string p_2)
        {
            WifiRemote.LogMessage("Not implemented yet for mp music", WifiRemote.LogType.Info);
        }

        internal static void ShowArtistDetails(string p)
        {
            WifiRemote.LogMessage("Not implemented yet for mp music", WifiRemote.LogType.Info);
        }

        internal static void ShowFileDetails(string p)
        {
            WifiRemote.LogMessage("Not implemented yet for mp music", WifiRemote.LogType.Info);
        }

        internal static void ShowFolderDetails(string p)
        {
            WifiRemote.LogMessage("Not implemented yet for mp music", WifiRemote.LogType.Info);
        }


        /// <summary>
        /// Add MpExtended information to playlist item
        /// </summary>
        /// <param name="item">MediaPortal playlist item</param>
        /// <param name="message">Playlist message item that is sent to client</param>
        internal static void AddMpExtendedInfo(MediaPortal.Playlists.PlayListItem item, PlaylistEntry message)
        {
            MusicDatabase mpMusicDb = MusicDatabase.Instance;
            Song song = new Song();
            bool inDb = mpMusicDb.GetSongByFileName(item.FileName, ref song);

            if (inDb)
            {
                message.Name2 = song.Album;
                message.AlbumArtist = song.AlbumArtist;
                message.Title = song.Title;
                message.MpExtId = song.Id.ToString();
                message.MpExtMediaType = (int)MpExtended.MpExtendedMediaTypes.MusicTrack;
                message.MpExtProviderId = (int)MpExtended.MpExtendedProviders.MPMusic;
            }
        }

        /// <summary>
        /// Create a playlist item given a track id
        /// </summary>
        /// <param name="trackId">track id</param>
        /// <returns>Playlist item</returns>
        internal static MediaPortal.Playlists.PlayListItem CreatePlaylistItemFromMusicTrack(int trackId)
        {
            PlayListItem item = null;

            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where idTrack=" + trackId;
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                item = PlaylistHelper.ToPlayListItem(songs[0]);
            }

            return item;
        }

        /// <summary>
        /// Create a playlist item given an album and artist
        /// </summary>
        /// <param param name="album">Album</param>
        /// <param name="albumArtist">Album Artist</param>
        /// <returns>Playlist items</returns>
        internal static List<PlayListItem> CreatePlaylistItemsFromMusicAlbum(string albumArtist, string album)
        {
            List<PlayListItem> returnList = new List<PlayListItem>();
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where strAlbumArtist like '%" + albumArtist + "%' AND strAlbum LIKE '%" + album + "%'";
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                foreach (Song s in songs)
                {
                    returnList.Add(PlaylistHelper.ToPlayListItem(s));
                }
            }
            return returnList;
        }

        /// <summary>
        /// Create a playlist item given a artist id
        /// </summary>
        /// <param name="albumArtist">Album Artist</param>
        /// <returns>Playlist items</returns>
        internal static List<PlayListItem> CreatePlaylistItemsFromMusicArtist(string albumArtist)
        {
            List<PlayListItem> returnList = new List<PlayListItem>();
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where strAlbumArtist like '%" + albumArtist + "%'"; 
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                foreach (Song s in songs)
                {
                    returnList.Add(PlaylistHelper.ToPlayListItem(s));
                }
            }
            return returnList;
        }
    }
}
