using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageCommand : IMessage
    {
        string type = "command";
        public string Type
        {
            get { return type; }
        }

        public String Command { get; set; }
    }
}
