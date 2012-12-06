using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.HardwareController.AudioController;
using MediaPortal.Configuration;

namespace WifiRemote.HardwareController
{
    class HardwareControllerFactory
    {
        private static String DefaultIdentifier = "MediaPortal";

        public static List<String> AvailableAudioControllers = new List<string> { "MediaPortal", "Pioneer VSX" };

        public static IAudioController AudioController()
        {
            using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
            {
                String audioControllerType = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "audioController", HardwareControllerFactory.DefaultIdentifier);

                if (audioControllerType == "Pioneer VSX")
                {
                    String ip = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "audioControllerIpAddress", "");
                    return new PioneerVSXAudioController(ip);
                }
            }

            // return default controller
            return new MediaPortalAudioController();
        }
    }
}
