using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageIdentify : IMessage
    {
        string type = "identify";
        public String Type
        {
            get { return type; }
        }

        public String Name
        {
            get { return System.Environment.MachineName; }
        }

        public String Application
        {
            get { return "WifiRemote Demo Client"; }
        }

        public String Version 
        {
            get { return "0.1"; }
        }

        public Authenticate Authenticate
        {
            get;
            set;
        }
    }

    class Authenticate
    {
        public String AuthMethod { get; set; }
        public String User { get; set; }
        public String Password { get; set; }
        public String PassCode { get; set; }

        public Authenticate(String theUser, String thePassword)
        {
            AuthMethod = "userpass";
            User = theUser;
            Password = thePassword;
        }

        public Authenticate(String thePasscode)
        {
            AuthMethod = "passcode";
            PassCode = thePasscode;
        }
    }
}
