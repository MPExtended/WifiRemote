using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class NowPlayingMusic : IAdditionalNowPlayingInfo
    {
        string mediaType = "music";
        public string MediaType
        {
            get { return mediaType; }
        }
    }
}
