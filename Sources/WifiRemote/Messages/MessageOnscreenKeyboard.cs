using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class MessageOnscreenKeyboard : IMessage
    {
        
        public string Type
        {
            get { return "onscreenkeyboard"; }
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
