using System;
using System.Collections.Generic;

namespace OctoAwesome.PoC
{
    public class CacheService
    {
        private readonly DependencyAgent dependencyAgent;

        private readonly Dictionary<Type, Cache> caches;
        public CacheService(DependencyAgent dependencyAgent)
        {
            this.dependencyAgent = dependencyAgent;
            caches = new();
        }

        public bool AddCache(Cache cache)
        {
            var type = cache.TypeOfTValue;
            return caches.TryAdd(type, cache);
        }

        public TValue Get<TKey, TValue>(TKey key)
        {
            if (!caches.TryGetValue(typeof(TValue), out var cache)
                || cache.TypeOfTKey != typeof(TKey))
            {
                return default;
            }

            //TValue Type C has to loaded
            //A and B has to be loaded before

            Dictionary<int, Type> types = dependencyAgent.GetDependencyTypeOrder(key, cache.TypeOfTKey, cache.TypeOfTValue);

            return cache.Get<TKey, TValue>(key);
        }

        //TODO: CleanupTask
    }
}
