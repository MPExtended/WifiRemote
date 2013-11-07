using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiRemote.HardwareController.AudioController;
using MediaPortal.Configuration;

namespace WifiRemote.HardwareController
{
    sealed class HardwareControllerFactory
    {
        private static readonly HardwareControllerFactory instance = new HardwareControllerFactory();

        private static String DefaultIdentifier = "MediaPortal";
        
        // Audio controllers
        public static List<String> AvailableAudioControllers = new List<string> { "MediaPortal", "Pioneer VSX" };
        private AbstractAudioController activeAudioController;

        // Singleton pattern
        static HardwareControllerFactory() { }

        private HardwareControllerFactory() { }

        public static HardwareControllerFactory Instance
        {
            get
            {
                return instance;
            }
        }

        // HardwareControllerFactory methods
        public AbstractAudioController AudioController()
        {
            if (activeAudioController == null)
            {
                // get selected audio controller from the settings
                using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(Config.GetFile(Config.Dir.Config, "MediaPortal.xml")))
                {
                    String audioControllerType = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "audioController", HardwareControllerFactory.DefaultIdentifier);

                    if (audioControllerType == "Pioneer VSX")
                    {
                        String ip = reader.GetValueAsString(WifiRemote.PLUGIN_NAME, "audioControllerIpAddress", "");
                        activeAudioController = new PioneerVSXAudioController(ip, 8102);
                    }
                }

                // fall back to default controller
                if (activeAudioController == null)
                {
                    activeAudioController = new MediaPortalAudioController();
                }
            }

            return activeAudioController;
        }
    }
}
