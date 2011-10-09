using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WifiRemote
{
    public class Property
    {
        public Property()
        {

        }

        public Property(string tag, string value)
        {
            this.Tag = tag;
            this.Value = value;
        }
        /// <summary>
        /// The key of the property
        /// </summary>
        public String Tag
        {
            get;
            set;
        }

        /// <summary>
        /// The value of the property
        /// </summary>
        public String Value
        {
            get;
            set;
        }
    }
}
