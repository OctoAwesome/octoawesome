using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome.Database
{
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo fileInfo;
        private FileStream? fileStream;
        public Writer(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }

        public Writer(string path) : this(new FileInfo(path))
        {

        }
        public void Open()
        {
            fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }
        public void Close()
        {
            fileStream?.Dispose();
            fileStream = null;
        }

        public void Write(ReadOnlySpan<byte> data)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Write(data);
        }
        public void Write(ReadOnlySpan<byte> data, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Seek(position, SeekOrigin.Begin);
            Write(data);
        }
        public void WriteAndFlush(ReadOnlySpan<byte> data)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data);
            fileStream.Flush();
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data, position);
            fileStream.Flush();
        }

        public void Write(ReadOnlySpan<byte> data, int offset, int length)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Write(data[offset..(offset + length)]);
        }
        public void Write(ReadOnlySpan<byte> data, int offset, int length, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Seek(position, SeekOrigin.Begin);
            Write(data[offset..(offset + length)]);
        }
        public void WriteAndFlush(ReadOnlySpan<byte> data, int offset, int length)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data[offset..(offset + length)]);
            fileStream.Flush();
        }

        public void WriteAndFlush(ReadOnlySpan<byte> data, int offset, int length, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data[offset..(offset + length)], position);
            fileStream.Flush();
        }

        internal long ToEnd()
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            return fileStream.Seek(0, SeekOrigin.End);
        }

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
