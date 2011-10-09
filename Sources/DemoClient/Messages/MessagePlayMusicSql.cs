using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient.Messages
{
    /// <summary>
    /// Start a channel on a client
    /// </summary>
    class MessagePlayMusicSql : IMessage
    {
        string type = "playlist";
        string playlistAction = "new";

        public MessagePlayMusicSql(string where, int limit, bool shuffle, bool autoplay, bool append)
        {
            PlayListSQL = new PlaylistSQL(where, limit);
            AutoPlay = autoplay;
            Shuffle = shuffle;
            if (append)
            {
                playlistAction = "append";
            }
        }

        public MessagePlayMusicSql(string playListName, bool shuffle, bool autoplay)
        {
            PlayListName = playListName;
            AutoPlay = autoplay;
            Shuffle = shuffle;
            playlistAction = "load";
        }

        public string Type
        {
            get { return type; }
        }

        public string PlayListName { get; set; }
        public PlaylistSQL PlayListSQL { get; set; }
        public bool AutoPlay { get; set; }
        public bool Shuffle { get; set; }
        public string PlaylistAction
        {
            get
            {
                return playlistAction;
            }
        }

        public string PlaylistType
        {
            get
            {
                return "music";
            }
        }

        public String AutologinKey
        {
            get;
            set;
        }
    }

    class PlaylistSQL
    {
        public string Where { get; set; }
        public int Limit { get; set; }

        public PlaylistSQL(string where, int limit)
        {
            Where = where;
            Limit = limit;
        }
    }
}
