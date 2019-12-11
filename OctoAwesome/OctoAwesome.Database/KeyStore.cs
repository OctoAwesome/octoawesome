using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITag, new()
    {
        public IEnumerable<TTag> Tags => keys.Keys;
        public IEnumerable<Key<TTag>> Keys => keys.Values;
        private readonly Dictionary<TTag, Key<TTag>> keys;
        private readonly Writer writer;
        private readonly Reader reader;

        public KeyStore(Writer writer, Reader reader)
        {
            keys = new Dictionary<TTag, Key<TTag>>();

            this.writer = writer;
            this.reader = reader;
        }

        public void Open()
        {
            writer.Open();
            var buffer = reader.Read(0, -1);

            for (int i = 0; i < buffer.Length; i += Key<TTag>.KEY_SIZE)
            {
                var key = Key<TTag>.FromBytes(buffer, i);
                keys.Add(key.Tag, key);
            }
        }

        internal Key<TTag> GetKey(TTag tag)
            => keys[tag];

        internal void Update(Key<TTag> key)
        {
            var oldKey = keys[key.Tag];
            keys[key.Tag] = new Key<TTag>(key.Tag, key.Index, key.Length, oldKey.Position);
            writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE, oldKey.Position);
        }

        internal bool Contains(TTag tag)
        {
            return keys.ContainsKey(tag);
        }

        internal void Add(Key<TTag> key)
        {
            key = new Key<TTag>(key.Tag, key.Index, key.Length, writer.ToEnd());
            keys.Add(key.Tag,  key);
            writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
        }

        internal void Remove(TTag tag, out Key<TTag> key)
        {
            key = keys[tag];
            keys.Remove(tag);
            writer.WriteAndFlush(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Position);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}
