using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    class MessageShowDialog : IMessage
    {
        String type = "showdialog";
        public String Type
        {
            get { return type; }
        }

        public String AutologinKey
        {
            get;
            set;
        }

        public String DialogType
        {
            get { return "ok"; }
        }

        public String Title
        {
            get { return "Message from DemoClient"; }
        }

        public String Text
        {
            get { return "This is an OK dialog. Oh myy!"; }
        }
    }
}
