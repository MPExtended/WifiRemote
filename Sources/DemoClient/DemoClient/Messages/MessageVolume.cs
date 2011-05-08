using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageVolume : IMessage
    {
        String type = "volume";
        public String Type
        {
            get { return type; }
        }

        public int Volume { get; set; }

        public MessageVolume(int vol)
        {
            Volume = vol;
        }

        public MessageVolume()
        {
            Volume = 0;
        }
    }
}
