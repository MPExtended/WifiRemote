using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    /// <summary>
    /// Class for holding basic information about this htpc
    /// </summary>
    public class ServerDescription
    {
        [Obsolete("Replaced by semicolon seperated adresses")]
        public String Address { get; set; }

        public String Addresses { get; set; }
        public int Port { get; set; }
        public String Hostname { get; set; }
        public String HardwareAddresses { get; set; }
        public String Name { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public String Passcode { get; set; }
        public int AuthOptions { get; set; }
    }
}
