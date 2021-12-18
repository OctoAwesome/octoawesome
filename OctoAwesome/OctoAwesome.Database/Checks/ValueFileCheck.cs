using System;
using System.IO;

namespace OctoAwesome.Database.Checks
{
    /// <summary>
    /// Helper class for checking value store files.
    /// </summary>
    /// <typeparam name="TTag">The identifying tag type for the value store.</typeparam>
    public sealed class ValueFileCheck<TTag> : ICheckable where TTag : ITag, new()
    {
        private readonly FileInfo fileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueFileCheck{TTag}"/> class.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> to the value store file to check.</param>
        public ValueFileCheck(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        /// <inheritdoc />
        public void Check()
        {
            using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];
            do
            {
                fileStream.Read(keyBuffer, 0, keyBuffer.Length);
                var key = Key<TTag>.FromBytes(keyBuffer, 0);

                if (!key.Validate())
                    throw new InvalidKeyException($"Key is not valid", fileStream.Position);

                if (key.Index != fileStream.Position - Key<TTag>.KEY_SIZE)
                    throw new InvalidKeyException($"Key is at the wrong position", fileStream.Position);

                int length;
                if (key.IsEmpty)
                {
                    var intBuffer = new byte[sizeof(int)];
                    fileStream.Read(intBuffer, 0, sizeof(int));
                    length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);
                }
                else
                {
                    length = key.ValueLength;
                }

                fileStream.Seek(length, SeekOrigin.Current);

            } while (fileStream.Position != fileStream.Length);
        }
    }
}
