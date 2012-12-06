using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;

namespace WifiRemote.HardwareController.AudioController
{
    class MediaPortalAudioController : IAudioController
    {
        public void VolumeUp()
        {
            VolumeHandler.Instance.Volume = VolumeHandler.Instance.Next;
        }

        public void VolumeDown()
        {
            VolumeHandler.Instance.Volume = VolumeHandler.Instance.Previous;
        }

        public void ToggleMute()
        {
            VolumeHandler.Instance.IsMuted = !VolumeHandler.Instance.IsMuted;
        }
    }
}
