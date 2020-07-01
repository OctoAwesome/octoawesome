using OctoAwesome.Database.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Database
{
    internal class KeyStore<TTag> : IDisposable where TTag : ITag, new()
    {
        public int EmptyKeys { get; private set; }
        public IReadOnlyList<TTag> Tags => keys.Keys.ToArray();
        public IReadOnlyList<Key<TTag>> Keys => keys.Values.ToArray();
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
            keys.Clear();
            EmptyKeys = 0;

            writer.Open();
            var buffer = reader.Read(0, -1);

            for (int i = 0; i < buffer.Length; i += Key<TTag>.KEY_SIZE)
            {
                var key = Key<TTag>.FromBytes(buffer, i);

                if (!key.Validate())
                    throw new KeyInvalidException("Key is not valid", i);

                if (key.IsEmpty)
                {
                    EmptyKeys++;
                    continue;
                }

                //TODO: Temp
                if (keys.ContainsKey(key.Tag))
                    continue;

                keys.Add(key.Tag, key);
            }
        }

        public void Close()
        {
            writer.Close();
        }

        internal Key<TTag> GetKey(TTag tag)
            => keys[tag];

        internal void Update(Key<TTag> key)
        {
            var oldKey = keys[key.Tag];
            keys[key.Tag] = new Key<TTag>(key.Tag, key.Index, key.ValueLength, oldKey.Position);
            writer.WriteAndFlush(key.GetBytes(), 0, Key<TTag>.KEY_SIZE, oldKey.Position);
        }

        internal bool Contains(TTag tag)
        {
            return keys.ContainsKey(tag);
        }

        internal void Add(Key<TTag> key)
        {
            key = new Key<TTag>(key.Tag, key.Index, key.ValueLength, writer.ToEnd());
            keys.Add(key.Tag, key);
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
            keys.Clear();
            writer.Dispose(); //TODO: Move to owner
        }
    }
}
