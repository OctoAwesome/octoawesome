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
        public abstract TValue Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists);

        internal abstract void Start();

        internal abstract void Stop();

        internal abstract void CollectGarbage();
    }

    public abstract class Cache<TKey, TValue> : Cache
    {
        public override Type TypeOfTValue { get; } = typeof(TValue);
        public override Type TypeOfTKey { get; } = typeof(TKey);

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);
        protected readonly CountedScopeSemaphore lockSemaphore = new();

        protected readonly Dictionary<TKey, CacheItem> valueCache = new();

        protected virtual TValue GetBy(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            CacheItem cacheItem;
            bool result;
            using (var @lock = lockSemaphore.EnterCountScope())
            {
                result = valueCache.TryGetValue(key, out cacheItem);
            }

            if (result
                && cacheItem.LastAccessTime.Add(ClearTime) < DateTime.Now)
            {
                cacheItem.LastAccessTime = DateTime.Now;
            }
            else if (loadingMode == LoadingMode.LoadIfNotExists)
            {
                var loadedValue = Load(key);
                cacheItem = new(loadedValue);

                using var @lock = lockSemaphore.EnterExclusivScope();
                valueCache[key] = cacheItem;
            }
            else if (loadingMode == LoadingMode.OnlyCached)
            {
                return default;
            }
            else
            {
                throw new NotSupportedException();
            }

            return cacheItem.Value;
        }

        protected abstract TValue Load(TKey key);

        protected CacheItem AddOrUpdate(TKey key, TValue value)
        {
            using var @lock = lockSemaphore.EnterExclusivScope();
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
                using var @lock = lockSemaphore.EnterExclusivScope();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    valueCache.Remove(element.Key, out _);
            }
        }

        internal virtual bool Remove(TKey key, out TValue value)
        {
            using var @lock = lockSemaphore.EnterExclusivScope();

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

        public override TV Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            return GenericCaster<TV, TValue>
                .Cast(
                    GetBy(GenericCaster<TKey, TK>.Cast(key), loadingMode)
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
