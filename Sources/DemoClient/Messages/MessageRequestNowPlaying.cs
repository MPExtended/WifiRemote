using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageRequestNowPlaying : IMessage
    {
        string type = "requestnowplaying";
        public string Type
        {
            get { return type; }
        }
    }
}
