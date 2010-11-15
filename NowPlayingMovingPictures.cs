using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class NowPlayingMovingPictures : IAdditionalNowPlayingInfo
    {
        string mediaType = "movie";
        public string MediaType
        {
            get { return mediaType; }
        }
    }
}
