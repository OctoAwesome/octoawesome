using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Package
    {
        public byte Type { get; set; }
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public Package(ushort command, int size, byte type = 0)
        {
            Type = type;
            Command = command;
            Payload = new byte[size];
        }
        public Package(byte[] data) : this(0, data.Length)
        {
            Write(data);
        }

        public void Write(byte[] buffer)
        {
            Type = buffer[0];
            Command = (ushort)(buffer[1] << 8 | buffer[2]);

            Array.Copy(buffer, 3, Payload, 0, buffer.Length - 3);
        }

        public int Read(byte[] buffer)
        {
            buffer[0] = Type;
            buffer[1] = (byte)(Command >> 8);
            buffer[2] = (byte)(Command & 0xF);

            Array.Copy(Payload, 0, buffer, 3, Payload.Length);

            return Payload.Length + 3;
        }

    }
}
