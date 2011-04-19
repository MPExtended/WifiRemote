using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class NowPlayingDVD : IAdditionalNowPlayingInfo
    {
        string mediaType = "dvd";
        public string MediaType
        {
            get { return mediaType; }
        }
    }
}
