using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Caching
{
    public class CacheService : IDisposable
    {
        private readonly Dictionary<Type, Cache> caches;
        private readonly IPlanet planet;
        private readonly IResourceManager resourceManager;
        private CancellationTokenSource cancellationTokenSource;
        private Task garbageCollectionTask;


        public CacheService(IPlanet planet, IResourceManager resourceManager, IUpdateHub updateHub)
        {
            caches = new Dictionary<Type, Cache>();

            this.planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            //TODO: In future we need a cool way to load all caches that explicit needed for this cache service
            var posCompCache = new PositionComponentCache(resourceManager);
            var chunkColumnCache = new ChunkColumnCache(resourceManager, planet);

            caches.Add(typeof(PositionComponent), posCompCache);
            caches.Add(typeof(List<PositionComponent>), posCompCache);
            caches.Add(typeof(ChunkColumn), chunkColumnCache);
            caches.Add(typeof(IChunkColumn), chunkColumnCache);
            caches.Add(typeof(Entity), new ComponentContainerCache<Entity, IEntityComponent>(resourceManager));
            caches.Add(typeof(FunctionalBlock), new ComponentContainerCache<FunctionalBlock, IFunctionalBlockComponent>(resourceManager));
        }
        
        public void Start()
        {
            if(cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            
            foreach (var item in caches.Values)
            {
                if (item.IsStarted)
                    continue;

                item.Start();
            }

            garbageCollectionTask = new Task(() => GarbageCollection(token), token, TaskCreationOptions.LongRunning);
            garbageCollectionTask.Start();

        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            foreach (var item in caches.Values)
            {
                if (!item.IsStarted)
                    continue;

                item.Stop();
            }
        }

        public void Dispose()
        {
            Stop();

            foreach (var item in caches.Values)
            {
                if(item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            caches.Clear();
        }

        public TValue Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
        {
            if(caches.TryGetValue(typeof(TValue), out var cache))
            {
                return cache.Get<TKey, TValue>(key, loadingMode);
            }
            else
            {
                return default;
            }
        }

        private void GarbageCollection(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (var item in caches.Values)
                {
                    item.CollectGarbage();
                }
            }
        }
    }
}
