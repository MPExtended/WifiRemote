using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Music.Database;
using WifiRemote.MPPlayList;

namespace WifiRemote.PluginConnection
{
    public class MpMusicHelper
    {
        public static void PlayMusicTrack(int _trackId, int _startPos)
        {
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where idTrack=" + _trackId;
            MusicDatabase.Instance.GetSongsByFilter(sql, out songs, "tracks");

            if (songs.Count > 0)
            {
                new Communication().PlayAudioFile(songs[0].FileName, _startPos);
            }
            WifiRemote.LogMessage("not implemented yet", WifiRemote.LogType.Warn);
        }

        public static void PlayAlbum(String albumArtist, String album, int startPos)
        {
            List<Song> songs = new List<Song>();
            string sql = "select * from tracks where strAlbumArtist like " + albumArtist + " AND strAlbum=" + album;
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
    }
}
