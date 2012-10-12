﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    interface IAdditionalNowPlayingInfo
    {
        /// <summary>
        /// Name of the current Media Type
        /// </summary>
        string MediaType
        {
            get;
        }

        /// <summary>
        /// MpExtended id of currently playing item
        /// </summary>
        string MpExtId
        {
            get;
        }

        /// <summary>
        /// MpExtended media type of currently playing item
        /// </summary>
        int MpExtMediaType
        {
            get;
        }

        /// <summary>
        /// MpExtended provider id of currently playing item
        /// </summary>
        int MpExtProviderId
        {
            get;
        }
    }
}
