using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MediaPortal.Player;

namespace WifiRemote
{
    /// <summary>
    /// Message containing information about the current volume on the htpc
    /// </summary>
    class MessageVolume : IMessage
    {
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
                try
                {
                    return VolumeHandler.Instance.Volume / (VolumeHandler.Instance.Maximum / 100);
                }
                catch (Exception)
                {
                    return 101;
                }
            }
        }

        /// <summary>
        /// Is the volume muted
        /// </summary>
        public bool IsMuted
        {
            get { return VolumeHandler.Instance.IsMuted; }
        }
    }
}
