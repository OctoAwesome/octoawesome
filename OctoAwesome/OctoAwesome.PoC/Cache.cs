using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

namespace OctoAwesome.PoC
{

    public abstract class Cache
    {
        public abstract Type TypeOfTValue { get; }
        public abstract Type TypeOfTKey { get; }
        public abstract TValue Get<TKey, TValue>(TKey key);

        internal abstract void CleanUp();
    }

    public abstract class Cache<TKey, TValue> : Cache
    {
        public override Type TypeOfTValue => typeof(TValue);
        public override Type TypeOfTKey => typeof(TKey);

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);

        private Dictionary<TKey, CacheItem> valueCache = new();

        protected TValue GetBy(TKey key)
        {
            if (valueCache.TryGetValue(key, out var value)
                && value.LastAccessTime.Add(ClearTime) < DateTime.Now)
            {
                value = value with { LastAccessTime = DateTime.Now };
            }
            else
            {
                var loadedValue = Load(key);
                value = value with { LastAccessTime = DateTime.Now, Value = loadedValue };
            }

            valueCache[key] = value;

            return value.Value;
        }

        protected abstract TValue Load(TKey key);

        internal override void CleanUp()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    valueCache.Remove(element.Key);
            }
        }

        internal bool Remove(TKey key)
        {
            return valueCache.Remove(key);
        }

        public override TV Get<TK, TV>(TK key)
        {
            return (TV)(object)GetBy((TKey)(object)key);
        }

        internal record CacheItem(DateTime LastAccessTime, TValue Value);
    }
}
