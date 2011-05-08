using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageIdentify : IMessage
    {
        string type = "identify";
        public String Type
        {
            get { return type; }
        }

        public String Name
        {
            get { return System.Environment.MachineName; }
        }

        public String Application
        {
            get { return "WifiRemote Demo Client"; }
        }

        public String Version 
        {
            get { return "0.1"; }
        }
    }
}
