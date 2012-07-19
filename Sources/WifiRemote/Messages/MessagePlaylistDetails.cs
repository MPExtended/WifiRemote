using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.MPPlayList;

namespace WifiRemote.Messages
{
    /// <summary>
    /// Represents a MP playlist
    /// </summary>
    public class MessagePlaylistDetails : IMessage
    {
        public string Type
        {
            get { return "playlistdetails"; }
        }

        /// <summary>
        /// Type of the playlist (currently supported: music, video)
        /// </summary>
        public String PlaylistType { get; set; }

        /// <summary>
        /// List of all items in this playlist
        /// </summary>
        public List<PlaylistEntry> PlaylistItems { get; set; }
    }
}
