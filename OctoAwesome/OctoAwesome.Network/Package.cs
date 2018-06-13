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

        public Package(ushort command, int size, byte type = 0) : this()
        {
            Type = type;
            Command = command;
            Payload = new byte[size];

            Zipped = size > 2000;


        }
        public Package()
        {

            memoryStream = new MemoryStream();
        }
        public Package(byte[] data) : this(0, data.Length)
        {
            throw new NotImplementedException();
            //TODO: fix method not found exception
            //Write(data, 0, data.Length);
        }

        public void Dispose()
        {
            memoryStream.Flush();
            memoryStream.Dispose();
        }
    }
}
