using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Database
{
    public sealed class Defragmentation<TTag> where TTag : ITag, new()
    {
        private readonly FileInfo keyStoreFile;
        private readonly FileInfo valueStoreFile;

        public Defragmentation(FileInfo keyStoreFile, FileInfo valueStoreFile)
        {
            this.keyStoreFile = keyStoreFile;
            this.valueStoreFile = valueStoreFile;
        }

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
            using (FileStream newValueStoreStream = newValueStoreFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (FileStream currentValueStoreStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    currentValueStoreStream.Read(keyBuffer, 0, keyBuffer.Length);
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);

                    if (key.IsEmpty)
                    {
                        var intBuffer = new byte[sizeof(int)];
                        currentValueStoreStream.Read(intBuffer, 0, sizeof(int));
                        var length = BitConverter.ToInt32(intBuffer, 0) - sizeof(int);

                        currentValueStoreStream.Seek(length, SeekOrigin.Current);
                    }
                    else
                    {
                        var buffer = new byte[key.ValueLength];
                        currentValueStoreStream.Read(buffer, 0, buffer.Length);
                        newValueStoreStream.Write(keyBuffer, 0, keyBuffer.Length);
                        newValueStoreStream.Write(buffer, 0, buffer.Length);
                        yield return key;
                    }
                } while (currentValueStoreStream.Position != currentValueStoreStream.Length);
            }
        }

        private IEnumerable<Key<TTag>> GetKeys(byte[] keyBuffer)
        {
            using (FileStream fileStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
            {
                do
                {
                    fileStream.Read(keyBuffer, 0, keyBuffer.Length);
                    var key = Key<TTag>.FromBytes(keyBuffer, 0);
                    long length = 0;

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
                    yield return key;
                } while (fileStream.Position != fileStream.Length);
            }
        }
    }
}
