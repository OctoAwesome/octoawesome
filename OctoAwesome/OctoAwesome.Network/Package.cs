using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Package
    {
        public const int HEAD_LENGTH = 4;

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

            if (buffer[3] == 1)
            {
                using (var memoryStream = new MemoryStream(buffer, HEAD_LENGTH, buffer.Length - HEAD_LENGTH))
                using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (var targetStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(targetStream);
                        Payload = targetStream.ToArray();
                    }
                }
            }
            else
            {
                Array.Copy(buffer, HEAD_LENGTH, Payload, 0, buffer.Length - HEAD_LENGTH);
            }

        }

        public int Read(byte[] buffer, bool zip = false)
        {
            buffer[0] = Type;
            buffer[1] = (byte)(Command >> 8);
            buffer[2] = (byte)(Command & 0xF);
            buffer[3] = (byte)(zip ? 1 : 0);

            if (zip)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                        gzipStream.Write(Payload, 0, Payload.Length);

                    var tmpBuffer = memoryStream.ToArray();
                    Array.Copy(tmpBuffer, 0, buffer, HEAD_LENGTH, tmpBuffer.Length);
                    return tmpBuffer.Length + HEAD_LENGTH;

                }
            }
            else
            {
                Array.Copy(Payload, 0, buffer, HEAD_LENGTH, Payload.Length);
            }

            return Payload.Length + HEAD_LENGTH;
        }

    }
}
