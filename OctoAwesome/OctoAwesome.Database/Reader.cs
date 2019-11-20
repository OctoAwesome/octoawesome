using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    public sealed class Reader
    {
        private readonly FileInfo fileInfo;

        public Reader(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }
        public Reader(string path) : this(new FileInfo(path))
        {

        }

        internal byte[] Read(long index, int length)
        {
            length = length < 0 ? (int)fileInfo.Length : length;

            var array = new byte[length];
            using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Seek(index, SeekOrigin.Begin);
                fileStream.Read(array, 0, length);
            }
            return array;
        }
    }
}
