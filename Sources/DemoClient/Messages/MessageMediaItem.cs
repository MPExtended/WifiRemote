using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageMediaItem : IMessage
    {
        public string Type
        {
            get { return "playmediaitem"; }
        }

        public string AutologinKey { get; set; }

        public string ItemId
        {
            get { return "playmovie"; }
        }

        public int MediaType
        {
            get { return 0; }
        }

        public int ProviderId
        {
            get { return 3; }
        }

        public Dictionary<string, string> PlayInfo
        {
            get
            {
                Dictionary<string, string> temp = new Dictionary<string, string>();
                temp.Add("Id", "9");
                return temp;
            }
        }
    }
}
