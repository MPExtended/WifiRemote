using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.MPFacade
{
    /// <summary>
    /// Item of a MediaPortal facade
    /// </summary>
    public class FacadeItem
    {
        /// <summary>
        /// First item of a facade list
        /// </summary>
        public String Label {get; set;}

        /// <summary>
        /// Second item of a facade list
        /// </summary>
        public String Label2 { get; set; }

        /// <summary>
        /// Third item of a facade list
        /// </summary>
        public String Label3 { get; set; }

        public FacadeItem(MediaPortal.GUI.Library.GUIListItem item)
        {
            this.Label = item.Label;
            this.Label2 = item.Label2;
            this.Label3 = item.Label3;
        }

        public FacadeItem(MediaPortal.GUI.Library.MenuButtonInfo item)
        {
            this.Label = item.Text;
        }
    }
}
