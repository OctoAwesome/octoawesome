using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Simple file writer for writing at specified positions.
    /// </summary>
    public sealed class Writer : IDisposable
    {
        private readonly FileInfo fileInfo;
        private FileStream? fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> to the file to write to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileInfo"/> is <c>null</c>.</exception>
        public Writer(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Writer"/> class.
        /// </summary>
        /// <param name="path">The path to the file to write to.</param>
        public Writer(string path) : this(new FileInfo(path))
        {

        }

        /// <summary>
        /// Opens the file to write to.
        /// </summary>
        public void Open()
        {
            fileStream = fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
        }

        /// <summary>
        /// Closes the file to write to.
        /// </summary>
        public void Close()
        {
            fileStream?.Dispose();
            fileStream = null;
        }

        /// <summary>
        /// Writes bytes to the file.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        public void Write(ReadOnlySpan<byte> data)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Write(data);
        }

        /// <summary>
        /// Writes bytes to the file at a specific position.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="position">The position to write to.</param>
        public void Write(ReadOnlySpan<byte> data, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Seek(position, SeekOrigin.Begin);
            Write(data);
        }

        /// <summary>
        /// Writes bytes to the file and flushes the stream.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <seealso cref="Write(System.ReadOnlySpan{byte})"/>
        public void WriteAndFlush(ReadOnlySpan<byte> data)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data);
            fileStream.Flush();
        }

        /// <summary>
        /// Writes bytes to the file at a specific position and flushes the stream.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="position">The position to write to.</param>
        /// <seealso cref="Write(System.ReadOnlySpan{byte}, long)"/>
        public void WriteAndFlush(ReadOnlySpan<byte> data, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data, position);
            fileStream.Flush();
        }

        /// <summary>
        /// Writes a slice of bytes to the file.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="offset">The offset of the slice to get for writing.</param>
        /// <param name="length">The length of the slice to get for writing.</param>
        public void Write(ReadOnlySpan<byte> data, int offset, int length)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Write(data[offset..(offset + length)]);
        }

        /// <summary>
        /// Writes a slice of bytes to the file at a specific position.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="offset">The offset of the slice to get for writing.</param>
        /// <param name="length">The length of the slice to get for writing.</param>
        /// <param name="position">The position to write to.</param>
        public void Write(ReadOnlySpan<byte> data, int offset, int length, long position)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            fileStream.Seek(position, SeekOrigin.Begin);
            Write(data[offset..(offset + length)]);
        }

        /// <summary>
        /// Writes a slice of bytes to the file and flushes the stream.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="offset">The offset of the slice to get for writing.</param>
        /// <param name="length">The length of the slice to get for writing.</param>
        /// <seealso cref="Write(System.ReadOnlySpan{byte}, int, int)"/>
        public void WriteAndFlush(ReadOnlySpan<byte> data, int offset, int length)
        {
            Debug.Assert(fileStream != null, nameof(fileStream) + " is not open!");
            Write(data[offset..(offset + length)]);
            fileStream.Flush();
        }

        /// <summary>
        /// Writes a slice of bytes to the file at a specific position and flushes the stream.
        /// </summary>
        /// <param name="data">The bytes to write.</param>
        /// <param name="offset">The offset of the slice to get for writing.</param>
        /// <param name="length">The length of the slice to get for writing.</param>
        /// <param name="position">The position to write to.</param>
        /// <seealso cref="Write(System.ReadOnlySpan{byte}, int, int, long)"/>
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


        /// <inheritdoc />
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
