using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.MPPlayList
{
    /// <summary>
    /// One item of a MP playlist
    /// </summary>
    public class PlaylistEntry
    {
        /// <summary>
        /// Name of the file that will get displayed in the playlist
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Full path to the file
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Duration of the file
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Indicates if the item has been played already
        /// </summary>
        public bool Played { get; set; }
    }
}
