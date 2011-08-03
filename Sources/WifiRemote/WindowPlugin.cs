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
            set { name = value; }
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

        public bool DisplayPlugin { get; set; }

        public WindowPlugin(string aName, int aWindowId, byte[] anIcon) : this(aName, aWindowId, anIcon, true)
        {
        }

        public WindowPlugin(string aName, int aWindowId, byte[] anIcon, bool display)
        {
            name = aName;
            windowId = aWindowId;
            icon = anIcon;
            DisplayPlugin = display;
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} (displayed: {2})", WindowId.ToString(), Name, DisplayPlugin.ToString());
        }
    }
}
