using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Video.Database;

namespace WifiRemote
{
    class NowPlayingVideo : IAdditionalNowPlayingInfo
    {
        string mediaType = "video";
        public string MediaType
        {
            get { return mediaType; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aMovie">The currently playing movie</param>
        public NowPlayingVideo(IMDBMovie aMovie)
        {
            
        }
    }
}
