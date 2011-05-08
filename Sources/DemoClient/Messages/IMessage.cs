using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoClient
{
    public interface IMessage
    {
        /// <summary>
        /// Type is a required attribute for all messages. 
        /// The client decides by this attribute what message was sent.
        /// </summary>
        String Type
        {
            get;
        }

    }
}
