using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deusty.Net;

namespace WifiRemote
{
    public class RemoteClient
    {
        public AsyncSocket Socket { get; set; }
        public List<String> Properties { get; set; }

        public RemoteClient(AsyncSocket theSocket)
        {
            Socket = theSocket;
        }
    }
}
