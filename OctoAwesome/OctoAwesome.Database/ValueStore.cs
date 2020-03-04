using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    internal class ValueStore : IDisposable
    {
        public bool Updateable { get;  }

        private readonly Writer writer;
        private readonly Reader reader;

        public ValueStore(Writer writer, Reader reader, bool updateable)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader)); 
            Updateable = updateable;
        }
        public ValueStore(Writer writer, Reader reader) : this(writer, reader, false)
        {

        }
        
        public Value GetValue<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            var byteArray = reader.Read(key.Index + Key<TTag>.KEY_SIZE, key.Length);
            return new Value(byteArray);
        }

        internal Key<TTag> AddValue<TTag>(TTag tag, Value value) where TTag : ITag, new()
        {
            var key = new Key<TTag>(tag, writer.ToEnd(), value.Content.Length);
            //TODO: Hash, Sync
            writer.Write(key.GetBytes(), 0, Key<TTag>.KEY_SIZE);
            writer.WriteAndFlush(value.Content, 0, value.Content.Length);
            return key;
        }

        internal void Remove<TTag>(Key<TTag> key) where TTag : ITag, new()
        {
            writer.Write(Key<TTag>.Empty.GetBytes(), 0, Key<TTag>.KEY_SIZE, key.Index);
            writer.WriteAndFlush(BitConverter.GetBytes(key.Length), 0, sizeof(int), key.Index + Key<TTag>.KEY_SIZE);
        }

        internal void Open()
        {
            writer.Open();
        }

        public void Dispose()
        {
            writer.Dispose();
        }
        
    }
}
