using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome
{
    public sealed class Cache<I, V>
    {
        private int size;

        private Dictionary<I, CacheItem> _cache;

        private Stopwatch watch = new Stopwatch();

        private Func<I, V> loadDelegate;

        private Action<I, V> saveDelegate;

        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public Cache(int size, Func<I, V> loadDelegate, Action<I, V> saveDelegate)
        {
            if (size < 1)
                throw new ArgumentException();

            if (loadDelegate == null)
                throw new ArgumentNullException();

            this.size = size;
            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;
            _cache = new Dictionary<I, CacheItem>(size + 1);
            watch.Start();
        }

        public V Get(I index)
        {
            CacheItem item = null;
            cacheLock.EnterReadLock();
            try
            {
                if (_cache.TryGetValue(index, out item))
                {
                    item.LastAccess = watch.Elapsed;
                    return item.Value;
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }

            if (item == null)
            {
                cacheLock.EnterWriteLock();
                try
                {
                    if (_cache.TryGetValue(index, out item))
                    {
                        item.LastAccess = watch.Elapsed;
                        return item.Value;
                    }

                    V result = loadDelegate(index);
                    if (result != null)
                    {
                        Set(index, result);
                        return result;
                    }
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }

            return default(V);
        }

        private void Set(I index, V value)
        {
            CacheItem item = new CacheItem()
            {
                Index = index,
                Value = value,
                LastAccess = watch.Elapsed
            };

            CacheItem toRemove = null;
            if (!_cache.ContainsKey(index))
            {
                _cache.Add(index, item);
            }

            if (_cache.Count > size)
            {
                toRemove = _cache.Values.OrderBy(v => v.LastAccess).First();
                _cache.Remove(toRemove.Index);
            }

            if (toRemove != null && saveDelegate != null)
                saveDelegate(toRemove.Index, toRemove.Value);
        }

        public void Flush()
        {
            cacheLock.EnterWriteLock();
            try
            {
                if (saveDelegate != null)
                {
                    foreach (var item in _cache.Values)
                    {
                        saveDelegate(item.Index, item.Value);
                    }
                }

                // _cache.Clear();
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        private class CacheItem
        {
            public TimeSpan LastAccess { get; set; }

            public I Index { get; set; }

            public V Value { get; set; }
        }
    }
}