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
    public class Package : IDisposable
    {
        /// <summary>
        /// Bytesize of Header
        /// </summary>
        public const int HEAD_LENGTH = 4;

        public byte Type { get; set; }
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        private MemoryStream memoryStream;
        private GZipStream gzipStream;

        public Package(ushort command, int size, byte type = 0)
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            memoryStream = new MemoryStream();
            gzipStream = new GZipStream(memoryStream, CompressionMode.Compress);
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

        private int position;
        public int Read(byte[] buffer, int offset, int count, bool zip = false)
        {
            int oldPosition = position;
            if (position == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;

                buffer[offset++] = Type;

                buffer[offset++] = (byte)(Command >> 8);

                buffer[offset++] = (byte)(Command & 0xF);

                buffer[offset++] = (byte)(zip ? 1 : 0);
                position += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

            if (zip)
            {
                if (memoryStream.Length == 0)
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gzipStream.Write(Payload, 0, Payload.Length);
                    }
                    memoryStream.Position = 0;
                }
                position += memoryStream.Read(buffer, offset, count);
            }
            else
            {
                var toCopy = Math.Min(count, Payload.Length - (position - HEAD_LENGTH));
                if (toCopy > 0)
                {
                    Array.Copy(Payload, position - HEAD_LENGTH, buffer, offset, toCopy);
                    position += toCopy;
                }
            }

            return position - oldPosition;
        }

        public void Dispose()
        {
            memoryStream.Flush();
            memoryStream.Dispose();
        }
    }
}
