using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.HardwareController.AudioController
{
    interface IAudioController
    {
        /// <summary>
        /// Increases the volume by one step
        /// </summary>
        void VolumeUp();

        /// <summary>
        /// Decreases the volume by one step
        /// </summary>
        void VolumeDown();

        /// <summary>
        /// Mutes the volume if it is not muted, unmutes it if it is
        /// </summary>
        void ToggleMute();

        /// <summary>
        /// Sets the volume of the client
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        /// <param name="relative">True if the volume should be changed relative to the current volume</param>
        void SetVolume(int volume, bool relative);
    }
}
