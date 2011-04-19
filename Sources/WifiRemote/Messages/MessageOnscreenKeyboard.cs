using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class MessageOnscreenKeyboard
    {
        string type = "onscreenkeyboard";
        public string Type
        {
            get { return type; }
        }

        bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public MessageOnscreenKeyboard(bool active)
        {
            IsActive = active;
        }
    }
}
