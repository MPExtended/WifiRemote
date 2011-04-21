using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    /// <summary>
    /// Message to be sent to the client if he requests all properties
    /// </summary>
    class MessageProperties : IMessage
    {
        public MessageProperties()
        {

        }

        /// <summary>
        /// Type of this message (properties)
        /// </summary>
        public String Type
        {
            get { return "properties"; }
        }

        /// <summary>
        /// A list of all relevant properties
        /// </summary>
        public List<Property> Tags
        {
            get;
            set;
        }
    }
}
