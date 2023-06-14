using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Service for managing multiple caches.
    /// </summary>
    public class CacheService : IDisposable
    {
        private readonly Dictionary<Type, Cache> caches;
        private readonly IPlanet planet;
        private readonly IResourceManager resourceManager;
        private CancellationTokenSource? cancellationTokenSource;
        private Task? garbageCollectionTask;


        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        /// <param name="planet">The planet to cache for.</param>
        /// <param name="resourceManager">The resource manager for loading resource assets.</param>
        /// <param name="updateHub">The update hub.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="planet"/> or <paramref name="resourceManager"/> were <c>null</c>.
        /// </exception>
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
        }

        private void Cancel()
        {
            if (cancellationTokenSource is not null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        /// <summary>
        /// Starts the service and all underlying caches.
        /// </summary>
        public void Start()
        {
            Cancel();

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            foreach (var item in caches.Values)
            {
                if (item.IsStarted)
                    continue;

                item.Start();
            }

            garbageCollectionTask = new Task(async () => await GarbageCollection(token), token, TaskCreationOptions.LongRunning);
            garbageCollectionTask.Start();

        }

        /// <summary>
        /// Stops the service and all underlying caches.
        /// </summary>
        public void Stop()
        {
            Cancel();

            foreach (var item in caches.Values)
            {
                if (!item.IsStarted)
                    continue;

                item.Stop();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Stop();

            foreach (var item in caches.Values)
            {
                if (item is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            caches.Clear();
        }

        /// <summary>
        /// Gets a value from the cache service.
        /// </summary>
        /// <typeparam name="TKey">The type of the identifying key to get the value by.</typeparam>
        /// <typeparam name="TValue">The type of the value to get the value of.</typeparam>
        /// <param name="key">The identifying key to get the value by.</param>
        /// <param name="loadingMode">Is ignored, because this cache load everything on startup and therefore is always cached</param>
        /// <returns>The value from the cache service.</returns>
        public TValue? Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            where TKey : notnull
        {
            if (caches.TryGetValue(typeof(TValue), out var cache))
            {
                return cache.Get<TKey, TValue>(key, loadingMode);
            }

            return default;
        }

        /// <summary>
        /// Gets a value from the cache service.
        /// </summary>
        /// <typeparam name="TKey">The type of the identifying key to get the value by.</typeparam>
        /// <typeparam name="TValue">The type of the value to get the value of.</typeparam>
        /// <param name="key">The identifying key to get the value by.</param>

        /// <returns>The value from the cache service.</returns>
        public void AddOrUpdate<TKey, TValue>(TKey key, TValue value)
            where TKey : notnull
        {
            if (caches.TryGetValue(typeof(TValue), out var cache))
            {
                cache.AddOrUpdate(key, value);
            }
        }

        private async Task GarbageCollection(CancellationToken cancellationToken)
        {
            try
            {

                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (var item in caches.Values)
                    {
                        item.CollectGarbage();
                    }

                    await Task.Delay(30000, cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
