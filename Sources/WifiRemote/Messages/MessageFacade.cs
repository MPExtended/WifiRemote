using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.MPPlayList;
using WifiRemote.MPFacade;

namespace WifiRemote.Messages
{
    /// <summary>
    /// Represents a MP facade
    /// </summary>
    public class MessageFacade : IMessage
    {
        public string Type
        {
            get { return "facade"; }
        }

        /// <summary>
        /// Currently shown view of the facade (e.g. list, thumb, ...)
        /// </summary>
        public String ViewType { get; set; }

        /// <summary>
        /// Id of the window where the facade is shown
        /// </summary>
        public int WindowId { get; set; }

        /// <summary>
        /// List of all items in this playlist
        /// </summary>
        public List<FacadeItem> FacadeItems { get; set; }
    }
}
