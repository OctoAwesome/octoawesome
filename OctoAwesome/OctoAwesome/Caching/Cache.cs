using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Base class for caching.
    /// </summary>
    public abstract class Cache
    {
        /// <summary>
        /// Gets the type of the cached items.
        /// </summary>
        public abstract Type TypeOfTValue { get; }
        /// <summary>
        /// Gets the type used as identifying keys.
        /// </summary>
        public abstract Type TypeOfTKey { get; }
        /// <summary>
        /// Gets a value indicating whether the caching was started.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets a value from the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the identifying key to get the value by.</typeparam>
        /// <typeparam name="TValue">The type of the value to get the value of.</typeparam>
        /// <param name="key">The identifying key to get the value by.</param>
        /// <param name="loadingMode">The <see cref="LoadingMode"/> used.</param>
        /// <returns>The value from the cache.</returns>
        public abstract TValue? Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            where TKey : notnull;

        public abstract void AddOrUpdate<TKey, TValue>(TKey key, TValue value) where TKey : notnull;

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

    /// <summary>
    /// Base class for caching of specific types.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifying key to get the cache value by.</typeparam>
    /// <typeparam name="TValue">The type of the cached items.</typeparam>
    public abstract class Cache<TKey, TValue> : Cache
        where TKey : notnull
    {
        /// <inheritdoc />
        public override Type TypeOfTValue { get; } = typeof(TValue);

        /// <inheritdoc />
        public override Type TypeOfTKey { get; } = typeof(TKey);

        /// <summary>
        /// Gets the time span to wait before an unused cache items gets garbage collected.
        /// </summary>
        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Semaphore for thread safety measures of cache.
        /// </summary>
        protected readonly CountedScopeSemaphore lockSemaphore = new();

        /// <summary>
        /// Holds the actual cache items.
        /// </summary>
        protected readonly Dictionary<TKey, CacheItem> valueCache = new();

        /// <summary>
        /// Gets a value from the cache.
        /// </summary>
        /// <param name="key">The identifying key to get the value by.</param>
        /// <param name="loadingMode">The <see cref="LoadingMode"/> used.</param>
        /// <returns>The value from the cache.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when <paramref name="loadingMode"/> has an invalid value.
        /// </exception>
        protected virtual TValue? GetBy(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            Debug.Assert(IsStarted, IsStarted + " == true");

            CacheItem? cacheItem;
            bool result;
            using (var @lock = lockSemaphore.EnterCountScope())
            {
                result = valueCache.TryGetValue(key, out cacheItem);
            }

            if (result
                && cacheItem!.LastAccessTime.Add(ClearTime) < DateTime.UtcNow)
            {
                cacheItem.LastAccessTime = DateTime.UtcNow;
            }

            if (result)
                return cacheItem!.Value;

            if (loadingMode == LoadingMode.LoadIfNotExists)
            {
                var loadedValue = Load(key);
                if (loadedValue is null)
                    return default;
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

        /// <summary>
        /// Loads a value from non-cache storage.
        /// </summary>
        /// <param name="key">The identifying key to load the value by.</param>
        /// <returns>The loaded value.</returns>
        protected abstract TValue? Load(TKey key);

        /// <summary>
        /// Add or update a cache item identified by the given key.
        /// </summary>
        /// <param name="key">The key to identify the cache item by.</param>
        /// <param name="value">The new value to cache.</param>
        /// <returns>The cache item created for the cached value.</returns>
        protected virtual CacheItem AddOrUpdateInternal(TKey key, TValue value)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();
            return valueCache[key] = new(value);
        }

        /// <summary>
        /// Add or update a cache item identified by the given key.
        /// </summary>
        /// <param name="key">The key to identify the cache item by.</param>
        /// <param name="value">The new value to cache.</param>
        public override void AddOrUpdate<TK, TV>(TK key, TV value)
        {
            _ = AddOrUpdateInternal(GenericCaster<TK, TKey>.Cast(key), GenericCaster<TV, TValue>.Cast(value));
        }

        internal override void CollectGarbage()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                using var @lock = lockSemaphore.EnterExclusiveScope();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.UtcNow)
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

        /// <inheritdoc />
        public override TV? Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            where TV : default
        {
            return GenericCaster<TValue, TV>
                .Cast(
                    GetBy(GenericCaster<TK, TKey>.Cast(key), loadingMode)
                );
        }

        /// <summary>
        /// Cached item.
        /// </summary>
        protected class CacheItem
        {
            internal DateTime LastAccessTime { get; set; }
            internal TValue Value { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheItem"/> class.
            /// </summary>
            /// <param name="value">The cached value.</param>
            public CacheItem(TValue value)
                : this(DateTime.UtcNow, value)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheItem"/> class.
            /// </summary>
            /// <param name="lastAccessTime">The last time the item was accessed.</param>
            /// <param name="value">The cached value.</param>
            public CacheItem(DateTime lastAccessTime, TValue value)
            {
                LastAccessTime = lastAccessTime;
                Value = value;
            }
        }
    }
}
