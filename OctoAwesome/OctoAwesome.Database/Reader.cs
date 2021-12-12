using System;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Reader
    {

        private readonly FileInfo fileInfo;
        public Reader(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }
        
        public Reader(string path) : this(new FileInfo(path))
        {

        }

        public byte[] Read(long position, int length)
        {
            if (length < 0)
            {
                fileInfo.Refresh();
                length = fileInfo.Exists ? (int)fileInfo.Length : length;
            }

            var array = new byte[length];
            using (var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fileStream.Seek(position, SeekOrigin.Begin);
                fileStream.Read(array, 0, length);
            }
            return array;
        }
    }
}
