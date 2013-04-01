using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageNotification : IMessage
    {
        public string Type
        {
            get { return "message"; }
        }

        public string AutologinKey { get; set; }

        public string Text
        {
            get { return "This is a test notification from WifiRemote!"; }
        }
    }
}
