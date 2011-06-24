using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient.Messages
{
    class MessagePosition : IMessage
    {
        String type = "position";
        public String Type
        {
            get { return type; }
        }

        public int SeekType { get; set; }
        public int Position { get; set; }

        public String AutologinKey
        {
            get;
            set;
        }
    }
}
