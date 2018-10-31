using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class OctoNetworkEventArgs : EventArgs
    {
        public OctoNetworkStream NetworkStream { get; set; }
        public int DataCount { get; set; }
        public BaseClient Client { get; internal set; }
    }
}
