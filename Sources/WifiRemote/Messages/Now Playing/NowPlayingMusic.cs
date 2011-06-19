using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Music.Database;

namespace WifiRemote
{
    class NowPlayingMusic : IAdditionalNowPlayingInfo
    {
        string mediaType = "music";
        public string MediaType
        {
            get { return mediaType; }
        }

        public string Album { get; set; }
        public string AlbumArtist { get; set; }
        public string Artist { get; set; }
        public int BitRate { get; set; }
        public string BitRateMode { get; set; }
        public int BPM { get; set; }
        public int Channels { get; set; }
        public string Codec { get; set; }
        public string Comment { get; set; }
        public string Composer { get; set; }
        public string Conductor { get; set; }
        public DateTime DateTimeModified { get; set; }
        public DateTime DateTimePlayed { get; set; }
        public int DiscId { get; set; }
        public int DiscTotal { get; set; }
        public int Duration { get; set; }
        public string Genre { get; set; }
        public string Lyrics { get; set; }
        public int Rating { get; set; }
        public int SampleRate { get; set; }
        public int TimesPlayed { get; set; }
        public string Title { get; set; }
        public int Track { get; set; }
        public int TrackTotal { get; set; }
        public string URL { get; set; }
        public string WebImage { get; set; }
        public int Year { get; set; }
        public string ImageName { get; set; }

        public NowPlayingMusic(Song song)
        {
            Album = song.Album;
            AlbumArtist = song.AlbumArtist;
            Artist = song.Artist;
            BitRate = song.BitRate;
            BitRateMode = song.BitRateMode;
            BPM = song.BPM;
            Channels = song.Channels;
            Codec = song.Codec;
            Comment = song.Comment;
            Composer = song.Composer;
            Conductor = song.Conductor;
            DateTimeModified = song.DateTimeModified;
            DateTimePlayed = song.DateTimePlayed;
            DiscId = song.DiscId;
            DiscTotal = song.DiscTotal;
            Duration = song.Duration;
            Genre = song.Genre;
            Lyrics = song.Lyrics;
            Rating = song.Rating;
            SampleRate = song.SampleRate;
            TimesPlayed = song.TimesPlayed;
            Title = song.Title;
            Track = song.Track;
            TrackTotal = song.TrackTotal;
            URL = song.URL;
            WebImage = song.WebImage;
            Year = song.Year;

            ImageName = MediaPortal.Util.Utils.GetAlbumThumbName(song.Artist, song.Album);
            ImageName = MediaPortal.Util.Utils.ConvertToLargeCoverArt(ImageName);
        }
    }
}
