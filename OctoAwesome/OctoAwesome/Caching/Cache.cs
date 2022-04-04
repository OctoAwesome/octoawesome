using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OctoAwesome.Caching
{

    public abstract class Cache
    {

        public abstract Type TypeOfTValue { get; }

        public abstract Type TypeOfTKey { get; }

        public bool IsStarted { get; private set; }

        public abstract TValue Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists);

        internal virtual void Start()
        {
            IsStarted = true;
        }

        internal virtual void Stop()
        {
            IsStarted = false;
        }

        internal abstract void CollectGarbage();
    }
    public abstract class Cache<TKey, TValue> : Cache
        where TKey : notnull
    {

        public override Type TypeOfTValue { get; } = typeof(TValue);
        public override Type TypeOfTKey { get; } = typeof(TKey);
        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);
        protected readonly CountedScopeSemaphore lockSemaphore = new();
        protected readonly Dictionary<TKey, CacheItem> valueCache = new();


        protected virtual TValue GetBy(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            Debug.Assert(IsStarted, IsStarted + " == true");

            CacheItem? cacheItem;
            bool result;
            using (var @lock = lockSemaphore.EnterCountScope())
            {
                result = valueCache.TryGetValue(key, out cacheItem);
            }

            if (result
                && cacheItem!.LastAccessTime.Add(ClearTime) < DateTime.Now)
            {
                cacheItem.LastAccessTime = DateTime.Now;
            }

            if (result)
                return cacheItem!.Value;

            if (loadingMode == LoadingMode.LoadIfNotExists)
            {
                var loadedValue = Load(key);
                cacheItem = new(loadedValue);

                using var @lock = lockSemaphore.EnterExclusiveScope();
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
            using var @lock = lockSemaphore.EnterExclusiveScope();
            return valueCache[key] = new(value);
        }


        internal override void CollectGarbage()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                using var @lock = lockSemaphore.EnterExclusiveScope();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    valueCache.Remove(element.Key, out _);
            }
        }

        internal virtual bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var returnValue
                = valueCache
                .Remove(key, out var cacheItem);

            value = returnValue ? cacheItem!.Value : default;

            return returnValue;
        }
        public override TV Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            return GenericCaster<TValue, TV>
                .Cast(
                    GetBy(GenericCaster<TK, TKey>.Cast(key), loadingMode)
                );
        }
        protected class CacheItem
        {
            internal DateTime LastAccessTime { get; set; }
            internal TValue Value { get; set; }

            public CacheItem(TValue value)
                : this(DateTime.Now, value)
            {
            }


            public CacheItem(DateTime lastAccessTime, TValue value)
            {
                LastAccessTime = lastAccessTime;
                Value = value;
            }
        }
    }
}
