using System;
using System.IO;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Simple file reader for reading at specified positions.
    /// </summary>
    public sealed class Reader
    {

        private readonly FileInfo fileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> to the file to read from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileInfo"/> is <c>null</c>.</exception>
        public Reader(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader"/> class.
        /// </summary>
        /// <param name="path">The path to the file to read from.</param>
        public Reader(string path) : this(new FileInfo(path))
        {

        }

        /// <summary>
        /// Reads bytes from the file at a specific position.
        /// </summary>
        /// <param name="position">The position to read from.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The bytes read from the file.</returns>
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
