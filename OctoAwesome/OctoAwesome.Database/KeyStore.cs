using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITagable
    {
        private readonly Dictionary<int, Key> keys;
        private readonly Writer writer;
        private readonly Reader reader;

        public KeyStore(Writer writer, Reader reader)
        {
            keys = new Dictionary<int, Key>();

            this.writer = writer;
            this.reader = reader;
        }

        public void Open()
        {
            writer.Open();
            var buffer = reader.Read(0, -1);

            for (int i = 0; i < buffer.Length; i += Key.KEY_SIZE)
            {
                var key = Key.FromBytes(buffer, i);
                keys.Add(key.Tag, key);
            }
        }

        internal Key GetKey(TTag tag) 
            => keys[tag.Tag];

        internal bool Contains(TTag tag)
        {
            return keys.ContainsKey(tag.Tag);
        }

        internal void Add(Key key)
        {
            keys.Add(key.Tag, key);
            writer.ToEnd();
            writer.WriteAndFlush(key.GetBytes(), 0, Key.KEY_SIZE);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
