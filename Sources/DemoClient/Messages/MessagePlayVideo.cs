using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessagePlayVideo : IMessage
    {
        String type = "playfile";
        public String Type
        {
            get { return type; }
        }

        public String AutologinKey
        {
            get;
            set;
        }

        public String FileType
        {
            get { return "video"; }
        }

        String filePath;
        public String Filepath
        {
            get;
            set;
        }

        public MessagePlayVideo(String filePath)
        {
            this.Filepath = filePath;
        }
    }
}
