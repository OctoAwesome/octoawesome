using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Helper class for defregmentation key store and value store files.
    /// </summary>
    /// <typeparam name="TTag">The type of the tag contained in the store files.</typeparam>
    public sealed class Defragmentation<TTag> where TTag : ITag, new()
    {
        private readonly FileInfo keyStoreFile;
        private readonly FileInfo valueStoreFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="Defragmentation{TTag}"/> class.
        /// </summary>
        /// <param name="keyStoreFile">The <see cref="FileInfo"/> to the key store file to defragment.</param>
        /// <param name="valueStoreFile">The <see cref="FileInfo"/> to the value store file to defragment.</param>
        public Defragmentation(FileInfo keyStoreFile, FileInfo valueStoreFile)
        {
            this.keyStoreFile = keyStoreFile;
            this.valueStoreFile = valueStoreFile;
        }

        /// <summary>
        /// Defragments the key and value store files.
        /// </summary>
        public void StartDefragmentation()
        {
            var newValueStoreFile = new FileInfo(Path.GetTempFileName());
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];

            IEnumerable<Key<TTag>> keys = DefragmentValues(newValueStoreFile, keyBuffer);

            keyStoreFile.Delete();
            WriteKeyFile(keys);

            valueStoreFile.Delete();
            newValueStoreFile.MoveTo(valueStoreFile.FullName);
        }

        /// <summary>
        /// Recreates the key tag file from the value store.
        /// </summary>
        public void RecreateKeyFile()
        {
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];

            IEnumerable<Key<TTag>> keys = GetKeys(keyBuffer);

            keyStoreFile.Delete();
            WriteKeyFile(keys);
        }

        private void WriteKeyFile(IEnumerable<Key<TTag>> keyList)
        {
            using (FileStream newKeyStoreFile = keyStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {

                foreach (Key<TTag> key in keyList)
                    newKeyStoreFile.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            }
        }

        private IEnumerable<Key<TTag>> DefragmentValues(FileInfo newValueStoreFile, byte[] keyBuffer)
        {
            bool TryReadInt(FileStream currentValueStoreStream, out int length)
            {
                Span<byte> intBuffer = stackalloc byte[sizeof(int)];
                if (currentValueStoreStream.Read(intBuffer) == 0)
                {
                    length = 0;
                    return false;
                }

                length = BitConverter.ToInt32(intBuffer) - sizeof(int);
                return true;
            }

            bool TryWriteValue(Key<TTag> key, FileStream currentValueStoreStream, FileStream newValueStoreStream)
            {
                Span<byte> buffer = stackalloc byte[key.ValueLength];
                if (currentValueStoreStream.Read(buffer) == 0)
                    return false;
                newValueStoreStream.Write(keyBuffer, 0, keyBuffer.Length);
                newValueStoreStream.Write(buffer);
                return true;
            }

            using (FileStream newValueStoreStream = newValueStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (FileStream currentValueStoreStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    if (currentValueStoreStream.Read(keyBuffer, 0, keyBuffer.Length) == 0)
                        break;
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);

                    if (key.IsEmpty)
                    {
                        if (!TryReadInt(currentValueStoreStream, out var length))
                            break;
                        if (length < 0)
                            throw new DataMisalignedException();
                        currentValueStoreStream.Seek(length, SeekOrigin.Current);
                    }
                    else
                    {
                        if (!TryWriteValue(key, currentValueStoreStream, newValueStoreStream))
                            break;
                        yield return key;
                    }
                } while (currentValueStoreStream.Position < currentValueStoreStream.Length);
            }
        }

        private IEnumerable<Key<TTag>> GetKeys(byte[] keyBuffer)
        {
            using (FileStream fileStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    if (fileStream.Read(keyBuffer, 0, keyBuffer.Length) == 0)
                        break;
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);
                    long length = 0;

                    if (key.IsEmpty)
                    {
                        Span<byte> intBuffer = stackalloc byte[sizeof(int)];
                        if (fileStream.Read(intBuffer) == 0)
                            break;
                        length = BitConverter.ToInt32(intBuffer) - sizeof(int);
                    }
                    else
                    {
                        length = key.ValueLength;
                    }
                    if (length < 0)
                        throw new DataMisalignedException();
                    fileStream.Seek(length, SeekOrigin.Current);
                    yield return key;
                } while (fileStream.Position < fileStream.Length);
            }
        }
    }
}
