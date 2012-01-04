using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.MPDialogs;

namespace WifiRemote.Messages
{
    public class MessageDialog : IMessage
    {
        public string Type
        {
            get { return "dialog"; }
        }

        /// <summary>
        /// Dialog data (only sent when dialog state == shown)
        /// </summary>
        public MpDialog Dialog { get; set; }

        /// <summary>
        /// Dialog state (shown, closed)
        /// </summary>
        public bool DialogShown { get; set; }

        /// <summary>
        /// True if the message contains updates to the already shown dialog, false if it's a new dialog
        /// </summary>
        public bool DialogUpdate { get; set; }
    }
}
