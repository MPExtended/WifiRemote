using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessagePlayTvSeries : IMessage
    {
        public string Type
        {
            get { return "tvseries"; }
        }

        public string AutologinKey { get; set; }

        public string Action
        {
            get { return "playseries"; }
        }

        /*
        public bool AskToResume
        {
            get { return false; }
        }
        */
        public int SeasonNumber
        {
            get { return 2; }
        }
        /*
        public int EpisodeNumber
        {
            get { return 1; }
        }
        */
        public string SeriesName
        {
            get { return "Burn Notice"; }
        }

        public bool SwitchToPlaylist
        {
            get
            {
                return true;
            }
        }

        public bool AutoStart
        {
            get { return true; }
        }

        public bool OnlyUnwatchedEpisodes
        {
            get { return true; }
        }
    }
}
