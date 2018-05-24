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
        public const int HEAD_LENGTH = 8;

        public byte Type { get; set; }
        public ushort Command { get; set; }

        public byte[] Payload { get; set; }

        public bool Zipped { get; set; }

        private int compressedSize;

        private MemoryStream memoryStream;
        private GZipStream gzipStream;

        private int readPosition;
        private int writePosition;

        public Package(ushort command, int size, byte type = 0)
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            Zipped = size > 2000;

            memoryStream = new MemoryStream();
            
        }
        public Package()
        {

        }
        public Package(byte[] data) : this(0, data.Length)
        {
            Write(data, 0, data.Length);
        }

        
        public int Write(byte[] buffer, int offset, int count)
        {
            int written = 0;
            if (writePosition == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;
                Type = buffer[offset];
                Command = (ushort)(buffer[offset + 1] << 8 | buffer[offset + 2]);
                compressedSize = buffer[offset + 3] << 24 | buffer[offset + 4] << 16 | buffer[offset + 5] << 8 | buffer[offset + 6];
                
                Zipped = buffer[offset + 7] == 1;
                if (Zipped)
                {
                    memoryStream.Position = 0;
                    gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress, true);
                }
                written = HEAD_LENGTH;
                writePosition += HEAD_LENGTH;
                offset += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

           
            if (Zipped)
            {
                var toCopy = Math.Min(count, compressedSize);
                memoryStream.Write(buffer, offset, toCopy);
                compressedSize -= toCopy;
                written += count;
                if (compressedSize == 0)
                {
                    Payload = memoryStream.ToArray();
                }
            }
            else
            {
                var toRead = Math.Min(count, compressedSize - writePosition - HEAD_LENGTH);
                Array.Copy(buffer, offset, Payload, writePosition - HEAD_LENGTH, toRead);
                writePosition += toRead;
                written += toRead;
            }
            return written;
        }

        
        public int Read(byte[] buffer, int offset, int count)
        {
            int oldPosition = readPosition;
            if (readPosition == 0)
            {
                if (count < HEAD_LENGTH)
                    return 0;

                buffer[offset++] = Type;

                buffer[offset++] = (byte)(Command >> 8);

                buffer[offset++] = (byte)(Command & 0xFF);

                buffer[offset++] = (byte)(compressedSize >> 24);
                buffer[offset++] = (byte)(compressedSize >> 16);
                buffer[offset++] = (byte)(compressedSize >> 8);
                buffer[offset++] = (byte)(compressedSize & 0xFF);

                buffer[offset++] = (byte)(Zipped ? 1 : 0);
                readPosition += HEAD_LENGTH;
                count -= HEAD_LENGTH;
            }

            if (Zipped)
            {
                if (memoryStream.Length == 0)
                {
                    using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                    {
                        gzipStream.Write(Payload, 0, Payload.Length);
                    }
                    memoryStream.Position = 0;
                    compressedSize = (int)memoryStream.Length;
                    buffer[offset++] = (byte)(compressedSize >> 24);
                    buffer[offset++] = (byte)(compressedSize >> 16);
                    buffer[offset++] = (byte)(compressedSize >> 8);
                    buffer[offset++] = (byte)(compressedSize & 0xFF);
                }
                readPosition += memoryStream.Read(buffer, offset, count);
            }
            else
            {
                var toCopy = Math.Min(count, Payload.Length - (readPosition - HEAD_LENGTH));
                if (toCopy > 0)
                {
                    Array.Copy(Payload, readPosition - HEAD_LENGTH, buffer, offset, toCopy);
                    readPosition += toCopy;
                }
            }

            return readPosition - oldPosition;
        }

        public void Dispose()
        {
            memoryStream.Flush();
            memoryStream.Dispose();
        }
    }
}
