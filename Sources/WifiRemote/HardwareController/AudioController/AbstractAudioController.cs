using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote.HardwareController.AudioController
{
    abstract class AbstractAudioController
    {
        /// <summary>
        /// An event that is broadcast when the audio controller status
        /// (volume, mute) changed.
        /// </summary>
        public event EventHandler OnAudioControllerStatusChangedEvent;

        /// <summary>
        /// Broadcasts an OnAudioControllerStatusChangedEvent.
        /// 
        /// Has to be called explicitly in AbstractAudioController 
        /// subclasses!
        /// </summary>
        /// <param name="e"></param>
        protected void OnAudioControllerStatusChanged(EventArgs e)
        {
            EventHandler handler = OnAudioControllerStatusChangedEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Increases the volume by one step
        /// </summary>
        abstract public void VolumeUp();

        /// <summary>
        /// Decreases the volume by one step
        /// </summary>
        abstract public void VolumeDown();

        /// <summary>
        /// Mutes the volume if it is not muted, unmutes it if it is
        /// </summary>
        abstract public void ToggleMute();

        /// <summary>
        /// Sets the volume of the client
        /// </summary>
        /// <param name="volume">The new volume, ranging from 0 to 100</param>
        /// <param name="relative">True if the volume should be changed relative to the current volume</param>
        abstract public void SetVolume(int volume, bool relative);

        /// <summary>
        /// Gets the current volume of the receiver
        /// </summary>
        /// <returns>The volume in percent</returns>
        abstract public int Volume();

        /// <summary>
        /// Returns if the receiver is muted
        /// </summary>
        /// <returns><code>true</code> of the receiver is muted</returns>
        abstract public bool IsMuted();
    }
}
