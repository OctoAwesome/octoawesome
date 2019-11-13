using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo fileInfo;
        private FileStream fileStream;

        public Writer(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }
        public Writer(string path) : this(new FileInfo(path))
        {

        }

        public void Open()
        {
           fileStream =  fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }

        public void Write(byte[] data, int offset, int length)
            => fileStream.Write(data, offset, length);

        #region IDisposable Support
        private bool disposedValue = false;
               

        public void Dispose()
        {
            if (disposedValue)
                return;

            fileStream?.Dispose();

            disposedValue = true;
        }
        #endregion

    }
}
