using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient.Messages
{
    /// <summary>
    /// Start a channel on a client
    /// </summary>
    class MessageStartChannel : IMessage
    {
        string type = "playchannel";

        public MessageStartChannel(int _channnel, bool _fullscreen)
        {
            this.ChannelId = _channnel;
            this.StartFullscreen = _fullscreen;
        }
        public string Type
        {
            get { return type; }
        }

        public int ChannelId { get; set; }
        public bool StartFullscreen { get; set; }

        public String AutologinKey
        {
            get;
            set;
        }
    }
}
