using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageScreenshot : IMessage
    {
        string type = "screenshot";
        public string Type
        {
            get { return type; }
        }

        public int Width
        {
            get { return 800; }
        }

        public String AutologinKey
        {
            get;
            set;
        }
    }
}
