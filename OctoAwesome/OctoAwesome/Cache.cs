using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class Cache<I, V>
    {
        private int size;

        // private Dictionary<I, CacheItem> _cache;

        private ConcurrentDictionary<I, CacheItem> _cache;

        private Stopwatch watch = new Stopwatch();

        private Func<I, V> loadDelegate;

        private Action<I, V> saveDelegate;

        public Cache(int size, Func<I, V> loadDelegate, Action<I, V> saveDelegate)
        {
            if (size < 1)
                throw new ArgumentException();

            if (loadDelegate == null)
                throw new ArgumentNullException();

            this.size = size;
            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;
            _cache = new ConcurrentDictionary<I, CacheItem>(2, size + 1);
            watch.Start();
        }

        public V Get(I index)
        {
            CacheItem item = null;

            // Cache prüfen
            //lock (_cache)
            //{
            if (_cache.TryGetValue(index, out item))
            {
                item.LastAccess = watch.Elapsed;
                return item.Value;
            }
            //}

            V result = loadDelegate(index);
            if (result != null)
            {
                item = new CacheItem()
                {
                    Index = index,
                    Value = result,
                    LastAccess = watch.Elapsed
                };

                CacheItem toRemove = null;
                //lock (_cache)
                //{
                if (!_cache.ContainsKey(index))
                {
                    _cache.AddOrUpdate(index, item, (i, value) =>
                    {
                        return value;
                    });
                }

                if (_cache.Count > size)
                {
                    toRemove = _cache.Values.OrderBy(v => v.LastAccess).First();
                    _cache.TryRemove(toRemove.Index, out toRemove);
                }
                //}

                if (toRemove != null && saveDelegate != null)
                    saveDelegate(toRemove.Index, toRemove.Value);
            }

            return default(V);
        }

        public void Set(I index, V value)
        {

        }

        public void Flush()
        {
            if (saveDelegate != null)
            {
                foreach (var item in _cache.Values)
                {
                    saveDelegate(item.Index, item.Value);
                }
            }

            _cache.Clear();
        }

        private class CacheItem
        {
            public TimeSpan LastAccess { get; set; }

            public I Index { get; set; }

            public V Value { get; set; }
        }
    }
}
