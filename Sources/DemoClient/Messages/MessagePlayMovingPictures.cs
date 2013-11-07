using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessagePlayMovingPictures : IMessage
    {
        public string Type
        {
            get { return "movingpictures"; }
        }

        public string AutologinKey { get; set; }

        public string Action
        {
            get { return "playmovie"; }
        }

        public bool AskToResume
        {
            get { return false; }
        }

        public string MovieName
        {
            get { return "Source Code"; }
        }
    }
}
