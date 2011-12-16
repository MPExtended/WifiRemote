using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Music.Database;
using WifiRemote.MPPlayList;
using System.IO;

namespace WifiRemote.PluginConnection
{
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
            string sql = "select * from tracks where strAlbumArtist like '%" + albumArtist + "%' AND strAlbum LIKE '%" + album + "%'";
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                PlaylistHelper.ClearPlaylist("music");
                int index = 0;
                foreach(Song s in songs)
                {

                    PlaylistEntry entry = new PlaylistEntry();
                    entry.FileName = s.FileName;
                    entry.Name = s.Title;
                    entry.Duration = s.Duration;
                    PlaylistHelper.AddSongToPlaylist("music", entry, index);
                    index++;
                }

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
                PlaylistHelper.ClearPlaylist("music");
                int index = 0;
                foreach (Song s in songs)
                {

                    PlaylistEntry entry = new PlaylistEntry();
                    entry.FileName = s.FileName;
                    entry.Name = s.Title;
                    entry.Duration = s.Duration;
                    PlaylistHelper.AddSongToPlaylist("music", entry, index);
                    index++;
                }

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
                PlaylistHelper.ClearPlaylist("music");

                int index = 0;
                foreach (String f in Directory.GetFiles(folder))
                {
                    if (isValidExtension(f, extensions))
                    {
                        PlaylistHelper.AddSongToPlaylist("music", f, index);
                        index++;
                    }
                }

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
    }
}
