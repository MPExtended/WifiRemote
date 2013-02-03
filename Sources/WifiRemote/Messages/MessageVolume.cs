using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;
using WifiRemote.HardwareController;
using WifiRemote.HardwareController.AudioController;

namespace WifiRemote
{
    /// <summary>
    /// Message containing information about the current volume on the htpc
    /// </summary>
    class MessageVolume : IMessage
    {
        private AbstractAudioController audioController;
 
        public string Type
        {
            get { return "volume"; }
        }

        /// <summary>
        /// Current volume in percent
        /// </summary>
        public int Volume
        {
            get
            {
                return audioController.Volume();
            }
        }

        /// <summary>
        /// Is the volume muted
        /// </summary>
        public bool IsMuted
        {
            get { return audioController.IsMuted(); }
        }

        /// <summary>
        /// Constructor. Get AudioController instance
        /// </summary>
        public MessageVolume()
        {
            audioController = HardwareControllerFactory.Instance.AudioController();
        }
    }
}
