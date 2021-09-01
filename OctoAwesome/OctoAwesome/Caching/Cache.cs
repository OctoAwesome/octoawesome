using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Caching
{
    public abstract class Cache
    {
        public abstract Type TypeOfTValue { get; }
        public abstract Type TypeOfTKey { get; }
        public abstract TValue Get<TKey, TValue>(TKey key);

        internal abstract void Initialize();

        internal abstract void CleanUp();
    }

    //public class PositionComponentCache : Cache<Guid, PositionComponent>
    //{
    //    //Planet
    //    //Coord

    //    //-> GetAll


    //}

    //public class ComponentContainerCache<TComponent> : Cache<Guid, ComponentContainer<TComponent>> where TComponent : IComponent
    //{
    //    protected override ComponentContainer<TComponent> Load(Guid key)
    //    {

    //        //DiskPersistanceManager.Load<TComponent>(out var component, ...);
    //    }
    //}

    public abstract class Cache<TKey, TValue> : Cache
    {
        public override Type TypeOfTValue { get; } = typeof(TValue);
        public override Type TypeOfTKey { get; } = typeof(TKey);

        protected TimeSpan ClearTime { get; set; } = TimeSpan.FromMinutes(15);

        private readonly Dictionary<TKey, CacheItem> valueCache = new();

        protected virtual TValue GetBy(TKey key)
        {
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
            => valueCache[key] = new(value);

        internal override void Initialize()
        {
        }

        internal override void CleanUp()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                    Remove(element.Key, out _);
            }
        }

        internal virtual bool Remove(TKey key, out TValue value)
        {
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
