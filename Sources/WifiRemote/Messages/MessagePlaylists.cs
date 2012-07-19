using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.Messages
{
	class MessagePlaylists : IMessage
    {
        public string Type
        {
            get { return "playlists"; }
        }

        /// <summary>
        /// List of available playlists
        /// </summary>
        public List<String> PlayLists { get; set; }
	}
}
