using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    public class Database<TTag> : IDisposable where TTag : ITag, new()
    {
        private readonly KeyStore<TTag> keyStore;
        private readonly ValueStore valueStore;

        public Database(FileInfo keyFile, FileInfo valueFile)
        {
            keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile));
        }

        public void Open()
        {
            keyStore.Open();
            valueStore.Open();
        }

        public Value GetValue(TTag tag)
        {
            var key = keyStore.GetKey(tag);
            return valueStore.GetValue(key);
        }

        public void AddOrUpdate(TTag tag, Value value)
        {
            var contains = keyStore.Contains(tag);
            if (contains)
            {
                var key = keyStore.GetKey(tag);
                valueStore.Remove(key);
            }

            var newKey = valueStore.AddValue(tag, value);

            if (contains)
                keyStore.Update(newKey);
            else
                keyStore.Add(newKey);
        }


        public bool ContainsKey(TTag tag)
            => keyStore.Contains(tag);


        public void Remove(TTag tag)
        {
            keyStore.Remove(tag, out var key);
            valueStore.Remove(key);
        }


        public void Dispose()
        {
            keyStore.Dispose();
            valueStore.Dispose();
        }
    }
}
