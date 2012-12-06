using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.HardwareController.AudioController
{
    interface IAudioController
    {
        void VolumeUp();
        void VolumeDown();
        void ToggleMute();
    }
}
