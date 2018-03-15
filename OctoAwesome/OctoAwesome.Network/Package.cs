using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Package
    {
        public byte[] Head { get; set; }
        public byte[] Payload { get; set; }

        public Package()
        {

        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
