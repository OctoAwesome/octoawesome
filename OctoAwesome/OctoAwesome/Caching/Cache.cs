using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Caching
{
    public abstract class Cache
    {
        public abstract Type TypeOfTValue { get; }
        public abstract Type TypeOfTKey { get; }
        public abstract TValue Get<TKey, TValue>(TKey key);

        internal abstract void Start();

        internal abstract void Stop();

        internal abstract void CollectGarbage();
    }

    public abstract class Cache<TKey, TValue> : Cache
    {
        public override Type TypeOfTValue { get; } = typeof(TValue);
        public override Type TypeOfTKey { get; } = typeof(TKey);

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);
        protected readonly LockSemaphore lockSemaphore = new(1,1);

        private readonly Dictionary<TKey, CacheItem> valueCache = new();

        protected virtual TValue GetBy(TKey key)
        {
            using var @lock = lockSemaphore.Wait();

            if (valueCache.TryGetValue(key, out var value)
                && value.LastAccessTime.Add(ClearTime) < DateTime.Now)
            {
                value.LastAccessTime = DateTime.Now;
            }
            else
            {
                var loadedValue = Load(key);
                value = new(loadedValue);
                valueCache[key] = value;
            }

            return value.Value;
        }

        protected abstract TValue Load(TKey key);

        protected CacheItem AddOrUpdate(TKey key, TValue value)
        {
            using var @lock = lockSemaphore.Wait();
            return valueCache[key] = new(value);
        }

        internal override void Start()
        {
        }

        internal override void Stop()
        {
        }

        internal override void CollectGarbage()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                using var @lock = lockSemaphore.Wait();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    valueCache.Remove(element.Key, out _);
            }
        }

        internal virtual bool Remove(TKey key, out TValue value)
        {
            using var @lock = lockSemaphore.Wait();

            var returnValue
                = valueCache
                .Remove(key, out var cacheItem);

            if (returnValue)
            {
                value = cacheItem!.Value;
            }
            else
            {
                value = default;
            }

            return returnValue;
        }

        public override TV Get<TK, TV>(TK key)
        {
            return GenericCaster<TV, TValue>
                .Cast(
                    GetBy(GenericCaster<TKey, TK>.Cast(key))
                );
        }

        protected class CacheItem
        {
            internal DateTime LastAccessTime { get; set; }
            internal TValue Value { get; set; }

            public CacheItem(TValue value)
            {
                LastAccessTime = DateTime.Now;
                Value = value;
            }
            public CacheItem(DateTime lastAccessTime, TValue value)
            {
                LastAccessTime = lastAccessTime;
                Value = value;
            }
        }
    }
}
