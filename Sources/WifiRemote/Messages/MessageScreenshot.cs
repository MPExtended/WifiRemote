using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MediaPortal.GUI.Library;
using System.Drawing;


namespace WifiRemote
{
    /// <summary>
    /// Sends a screenshot to the client that requested it with the
    /// screenshot command.
    /// </summary>
    class MessageScreenshot : IMessage
    {
        public string Type
        {
            get { return "screenshot"; }
        }

        byte[] screenshot = new byte[0];
        /// <summary>
        /// The requested screenshot as byte array
        /// </summary>
        public byte[] Screenshot
        {
            get;
            set;
        }

        public ImageHelperError Error { get; set; }
    }
}
