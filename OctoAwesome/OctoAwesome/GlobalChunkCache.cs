using OctoAwesome.Caching;
using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome
{
    /// <summary>
    /// Global cache for chunks.
    /// </summary>
    public sealed class GlobalChunkCache : IGlobalChunkCache, IDisposable
    {
        //public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        private readonly CancellationTokenSource tokenSource;
        private readonly IResourceManager resourceManager;
        private readonly IUpdateHub updateHub;
        private readonly LockSemaphore semaphore = new LockSemaphore(1, 1);
        private readonly LockSemaphore updateSemaphore = new LockSemaphore(1, 1);

        // TODO: Sooner or later outsource
        private readonly ILogger logger;
        private readonly ChunkPool chunkPool;
        private readonly IDisposable chunkSubscription;
        private readonly IDisposable chunkSource;
        private readonly IDisposable simulationSource;
        private readonly Relay<Notification> chunkRelay;
        private readonly Relay<Notification> simulationRelay;

        /// <inheritdoc />
        public int LoadedChunkColumns
        {
            get
            {
                using (semaphore.Wait())
                {
                    return 0; //TODO Get Real loaded chunk columns
                }
            }
        }

        /// <inheritdoc />
        public int DirtyChunkColumn => 0;

        /// <inheritdoc />
        public IPlanet Planet { get; }

        /// <inheritdoc />
        public CacheService CacheService => cacheService;

        private readonly CacheService cacheService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalChunkCache"/> class.
        /// </summary>
        /// <param name="planet">The planet the global chunk cache manages chunks for.</param>
        /// <param name="resourceManager">The current <see cref="IResourceManager"/> to load resources.</param>
        /// <param name="updateHub">The update hub to propagate updates.</param>
        /// <param name="serIdProvider">The type provider used to manage serialization types..</param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager, IUpdateHub updateHub)
        {
            cacheService = new CacheService(planet, resourceManager, updateHub);
            cacheService.Start();

            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            this.updateHub = updateHub;
            chunkRelay = new Relay<Notification>();
            simulationRelay = new Relay<Notification>();


            tokenSource = new CancellationTokenSource();
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            chunkPool = TypeContainer.Get<ChunkPool>();

            chunkSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(OnNext);
            chunkSource = updateHub.AddSource(chunkRelay, DefaultChannels.Chunk);
            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        /// <inheritdoc />
        public IChunkColumn Subscribe(Index2 position)
        {
            var column = cacheService.Get<Index2, ChunkColumn>(position)!;

            var positionComponents
                        = cacheService
                        .Get<Index2, List<PositionComponent>>(position)!;

            if (resourceManager.LocalPersistance)
            {
                foreach (var positionComponent in positionComponents)
                {
                    if (!SerializationIdTypeProvider.TryGet(positionComponent.ParentTypeId, out var type))
                    {
                        continue;
                    }

                    if (type.IsAssignableTo(typeof(Entity)))
                    {
                        var entity
                            = cacheService
                            .Get<Guid, Entity>(positionComponent.ParentId)!;
                        if (entity is null)
                            continue;
                        positionComponent.Parent = entity;

                        logger.Debug($"Send {entity.GetType().Name} with id {entity.Id} to simulation");
                        var notification = new EntityNotification
                        {
                            Entity = entity,
                            Type = EntityNotification.ActionType.Add
                        };

                        simulationRelay.OnNext(notification);
                    }
                }
            }

            return column;
        }

        /// <inheritdoc />
        public IChunkColumn? Peek(Index2 position)
            => cacheService.Get<Index2, ChunkColumn>(position, LoadingMode.OnlyCached);

        /// <inheritdoc />
        public void Release(Index2 position)
        {
            //using (semaphore.Wait())
            {
                //if (!cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                //{
                //    throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                //}

                //if (--cacheItem.References <= 0)
                //{
                //    if (cacheItem.References < 0)
                //        logger.Warn($"Remove Reference from {cacheItem.Index}, now at: {cacheItem.References}");

                //    _unreferencedItems.Enqueue(cacheItem);
                //    _autoResetEvent.Set();
                //}
            }
        }


        /// <inheritdoc />
        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (updateSemaphore)
            {
                //TODO Load and remove entities accordingly
                // Add new chunks to the simulation
                //chunk.ChunkColumn.ForEachEntity(simulation.Add);

                // Remove old
                //chunk.ChunkColumn.ForEachEntity(simulation.Remove);
            }
        }


        /// <summary>
        /// Called when chunk notification occurred.
        /// </summary>
        /// <param name="value">The notification.</param>
        public void OnNext(object value)
        {
            switch (value)
            {
                case BlockChangedNotification blockChangedNotification:
                    Update(blockChangedNotification);
                    break;
                case BlocksChangedNotification blocksChangedNotification:
                    Update(blocksChangedNotification);
                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc />
        public void OnUpdate(SerializableNotification notification)
        {
            updateHub.PushNetwork(notification, DefaultChannels.Chunk);

            if (notification is IChunkNotification)
                chunkRelay.OnNext(notification);
        }

        /// <inheritdoc />
        public void Update(SerializableNotification notification)
        {
            if (notification is IChunkNotification chunk)
            {
                var column = cacheService.Get<Index3, ChunkColumn>(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet), LoadingMode.OnlyCached);
                if (column is null)
                    return;
                column.Update(notification);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            semaphore.Dispose();
            updateSemaphore.Dispose();
            autoResetEvent.Dispose();
            chunkSubscription.Dispose();
            chunkSource.Dispose();
            tokenSource.Dispose();
            chunkRelay.Dispose();

            cacheService.Dispose();
        }

        /// <inheritdoc />
        public void AfterSimulationUpdate(Simulation simulation)
        {
        }
    }
}
