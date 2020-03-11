using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
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

        public void Start()
        {
            var newValueStoreFile = new FileInfo(Path.GetTempFileName());
            var keyBuffer = new byte[Key<TTag>.KEY_SIZE];
            var keyList = new List<Key<TTag>>();

            using(var newValueStoreStream = newValueStoreFile.Open(FileMode.Open, FileAccess.Write, FileShare.None))
            using (var currentValueStoreStream = valueStoreFile.Open(FileMode.Open, FileAccess.Read, FileShare.None))
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
                        var buffer = new byte[key.Length];
                        currentValueStoreStream.Read(buffer, 0, buffer.Length);
                        newValueStoreStream.Write(keyBuffer, 0, keyBuffer.Length);
                        newValueStoreStream.Write(buffer, 0, buffer.Length);
                        keyList.Add(key);
                    }
                } while (currentValueStoreStream.Position != currentValueStoreStream.Length);
            }
        }
    }
}
