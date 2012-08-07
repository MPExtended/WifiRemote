using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.Messages
{
    class MessageDialogResult: IMessage
    {
        string type = "dialogResult";

        /// <summary>
        /// Type of this method
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Result of Yes/No dialog
        /// </summary>
        public bool YesNoResult { get; set; }

        /// <summary>
        /// Id of dialog (random id sent by client)
        /// </summary>
        public String DialogId { get; set; }

        /// <summary>
        /// Selected item
        /// </summary>
        public String SelectedOption { get; set; }
    }
}
