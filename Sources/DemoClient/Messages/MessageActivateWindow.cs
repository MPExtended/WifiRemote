using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageActivateWindow : IMessage
    {
        string type = "activatewindow";
        public string Type
        {
            get { return type; }
        }

        public String AutologinKey
        {
            get;
            set;
        }

        public int Window { get; set; }
        public string Parameter { get; set; }
    }
}
