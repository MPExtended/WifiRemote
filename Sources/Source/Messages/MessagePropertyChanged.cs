using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;
using MediaPortal.GUI.Library;

namespace WifiRemote
{
    /// <summary>
    /// Message to be sent if a property has changed 
    /// </summary>
    class MessagePropertyChanged : Property
    {
        string type = "propertychanged";
  
        public MessagePropertyChanged()
        {

        }
        public MessagePropertyChanged(string tag, string tagValue)
        {
            this.Tag = tag;
            this.Value = tagValue;
        }
        public String Type
        {
            get { return type; }
        }
    }
}
