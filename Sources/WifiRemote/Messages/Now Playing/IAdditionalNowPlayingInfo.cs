using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    interface IAdditionalNowPlayingInfo
    {
        string MediaType
        {
            get;
        }

        string MpExtId
        {
            get;
        }

        int MpExtMediaType
        {
            get;
        }

        int MpExtProviderId
        {
            get;
        }
    }
}
