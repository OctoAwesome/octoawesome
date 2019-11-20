using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctoAwesome.Database
{
    public class Database<TTag> : IDisposable where TTag : ITagable
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
            if (keyStore.Contains(tag))
            {
                //TODO: Update
            }
            else
            {
                keyStore.Add(valueStore.AddValue(tag, value));
            }            
        }

        public void Remove(TTag tag) { }

        public void Dispose()
        {
            keyStore.Dispose();
            valueStore.Dispose();
        }
    }
}
