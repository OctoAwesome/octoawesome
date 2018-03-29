using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Package
    {
        public byte Type { get; set; }
        public byte[] Payload { get; set; }

        public Package(int size, byte type = 0)
        {
            Type = type;
            Payload = new byte[size];
        }

        public void Write(byte[] buffer, int length)
        {
            Array.Copy(buffer, Payload, length);
        }

        public int Read(byte[] buffer, int count)
        {
            Array.Copy(Payload, buffer, count);

            return -1; //TODO
        }

    }
}
