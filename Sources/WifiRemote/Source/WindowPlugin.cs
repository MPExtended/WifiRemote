using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    class WindowPlugin
    {
        string name;
        public string Name
        {
            get { return name; }
        }

        int windowId;
        public int WindowId
        {
            get { return windowId; }
        }

        byte[] icon;
        public byte[] Icon
        {
            get { return icon; }
        }

        public WindowPlugin(string aName, int aWindowId, byte[] anIcon)
        {
            name = aName;
            windowId = aWindowId;
            icon = anIcon;
        }
    }
}
