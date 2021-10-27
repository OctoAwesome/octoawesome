using OctoAwesome.Caching;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Globaler Cache für Chunks
    /// </summary>
    public sealed class GlobalChunkCache : IGlobalChunkCache, IDisposable
    {
        //public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly CancellationTokenSource tokenSource;
        private readonly IResourceManager resourceManager;
        private readonly SerializationIdTypeProvider typeProvider;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly LockSemaphore semaphore = new LockSemaphore(1, 1);
        private readonly LockSemaphore updateSemaphore = new LockSemaphore(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task cleanupTask;
        private readonly ILogger logger;
        private readonly ChunkPool chunkPool;
        private readonly IDisposable chunkSubscription;
        private readonly IDisposable networkSource;
        private readonly IDisposable chunkSource;
        private readonly IDisposable simulationSource;
        private readonly Relay<Notification> networkRelay;
        private readonly Relay<Notification> chunkRelay;
        private readonly Relay<Notification> simulationRelay;

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
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

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => 0;

        public IPlanet Planet { get; }

        private readonly CacheService cacheService;

        /// <summary>
        /// Create new instance of GlobalChunkCache
        /// </summary>
        /// <param name="resourceManager">the current <see cref="IResourceManager"/> to load ressources/></param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager, IUpdateHub updateHub, SerializationIdTypeProvider typeProvider)
        {
            cacheService = new CacheService(planet, resourceManager, updateHub);
            cacheService.Start();

            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            this.typeProvider = typeProvider;
            networkRelay = new Relay<Notification>();
            chunkRelay = new Relay<Notification>();
            simulationRelay = new Relay<Notification>();


            tokenSource = new CancellationTokenSource();
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            chunkPool = TypeContainer.Get<ChunkPool>();

            chunkSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(OnNext);
            networkSource = updateHub.AddSource(networkRelay, DefaultChannels.Network);
            chunkSource = updateHub.AddSource(chunkRelay, DefaultChannels.Chunk);
            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(Index2 position)
        {
            var column = cacheService.Get<Index2, ChunkColumn>(position);
            var chunkIndex = new Index3(position, Planet.Id);

            var positionComponents
                        = cacheService
                        .Get<Index3, List<PositionComponent>>(chunkIndex);

            foreach (var positionComponent in positionComponents)
            {
                if (!typeProvider.TryGet(positionComponent.InstanceTypeId, out var type))
                    continue;

                if (type.IsAssignableTo(typeof(Entity)))
                {
                    var entity
                        = cacheService
                        .Get<Guid, Entity>(positionComponent.InstanceId);

                    positionComponent.SetInstance(entity);
                    var notification = new EntityNotification
                    {
                        Entity = entity,
                        Type = EntityNotification.ActionType.Add
                    };

                    simulationRelay.OnNext(notification);
                }

                if (type.IsAssignableTo(typeof(FunctionalBlock)))
                {
                    var functionalBlock
                        = cacheService
                        .Get<Guid, FunctionalBlock>(positionComponent.InstanceId);
                    if (functionalBlock.Components.TryGetComponent<PositionComponent>(out var poscomp))
                        Debug.WriteLine(poscomp.Position.ToString());
                    positionComponent.SetInstance(functionalBlock);
                    var notification = new FunctionalBlockNotification
                    {
                        Block = functionalBlock,
                        Type = FunctionalBlockNotification.ActionType.Add
                    };

                    simulationRelay.OnNext(notification);
                }
            }

            return column;
        }


        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(Index2 position)
            => cacheService.Get<Index2, ChunkColumn>(position, LoadingMode.OnlyCached);



        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
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



        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (updateSemaphore)
            {
                //TODO Load and remove entities accordingly
                //Neue Chunks in die Simulation einpflegen
                //chunk.ChunkColumn.ForEachEntity(simulation.Add);

                //Alte Chunks aus der Siumaltion entfernen
                //chunk.ChunkColumn.ForEachEntity(simulation.Remove);
            }
        }


        public void OnNext(Notification value)
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

        public void OnUpdate(SerializableNotification notification)
        {
            networkRelay.OnNext(notification);

            if (notification is IChunkNotification)
                chunkRelay.OnNext(notification);
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is IChunkNotification chunk)
            {
                var column = cacheService.Get<Index3, ChunkColumn>(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet), LoadingMode.OnlyCached);
                if (column is null)
                    return;
                column?.Update(notification);
            }
        }

        public void Dispose()
        {
            semaphore.Dispose();
            updateSemaphore.Dispose();
            _autoResetEvent.Dispose();
            chunkSubscription.Dispose();
            networkSource.Dispose();
            chunkSource.Dispose();
            tokenSource.Dispose();
            networkRelay?.Dispose();
            chunkRelay?.Dispose();

            cacheService.Dispose();
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
        }
    }
}
