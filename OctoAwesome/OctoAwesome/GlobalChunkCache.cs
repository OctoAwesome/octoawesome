﻿using OctoAwesome.Caching;
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
        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly ConcurrentQueue<CacheItem> _unreferencedItems = new ConcurrentQueue<CacheItem>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private readonly Dictionary<Index3, CacheItem> cache;
        private readonly Queue<CacheItem> newChunks;
        private readonly Queue<CacheItem> oldChunks;
        private readonly CancellationTokenSource tokenSource;
        private readonly IResourceManager resourceManager;

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

        private readonly Relay<Notification> networkRelay;
        private readonly Relay<Notification> chunkRelay;

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                using (semaphore.Wait())
                {
                    return cache.Count;
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
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager, IUpdateHub updateHub)
        {
            cacheService = new CacheService(planet, resourceManager, updateHub);
            cacheService.Start();

            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            cache = new Dictionary<Index3, CacheItem>();
            newChunks = new Queue<CacheItem>();
            oldChunks = new Queue<CacheItem>();

            networkRelay = new Relay<Notification>();
            chunkRelay = new Relay<Notification>();

            tokenSource = new CancellationTokenSource();
            cleanupTask = new Task(async () => await BackgroundCleanup(tokenSource.Token), TaskCreationOptions.LongRunning);
            cleanupTask.Start(TaskScheduler.Default);
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(GlobalChunkCache));

            chunkPool = TypeContainer.Get<ChunkPool>();

            chunkSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(OnNext);
            networkSource = updateHub.AddSource(networkRelay, DefaultChannels.Network);
            chunkSource = updateHub.AddSource(chunkRelay, DefaultChannels.Chunk);
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(Index2 position)
        {
            CacheItem cacheItem = null;

            using (semaphore.Wait())
            {

                if (!cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
                {

                    cacheItem = new CacheItem(chunkPool)
                    {
                        Planet = Planet,
                        Index = position,
                        References = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    //_dirtyItems.Enqueue(cacheItem);
                    cache.Add(new Index3(position, Planet.Id), cacheItem);
                    //_autoResetEvent.Set();
                }
                cacheItem.References++;

                if (cacheItem.References > 1)
                    logger.Warn($"Add Reference to:{cacheItem.Index}, now at:{cacheItem.References}");

            }

            using (cacheItem.Wait())
            {

                if (cacheItem.ChunkColumn == null)
                {
                    //using (cacheItem.Wait())
                    //{
                    cacheItem.ChunkColumn = resourceManager.LoadChunkColumn(Planet, position);
                    var chunkIndex = new Index3(position, Planet.Id);
                    var positionComponents
                        = cacheService
                        .Get<Index3, List<PositionComponent>>(chunkIndex);
                    //TODO TypeIdProvider for the new SerializationId
                    foreach (var positionComponent in positionComponents)
                    {

                        if (positionComponent.Instance is Entity entity)
                            cacheItem.ChunkColumn.Add(resourceManager.LoadComponentContainer<Entity, IEntityComponent>(entity.Id));
                        //else if(positionComponent.Instance is FunctionalBlock functionalBlock)
                        //cacheItem.ChunkColumn.Add(resourceManager.LoadComponentContainer<FunctionalBlock, IFunctionalBlockComponent>(functionalBlock.Id));
                    }

                    using (updateSemaphore.Wait())
                        newChunks.Enqueue(cacheItem);

                    //}
                }

                return cacheItem.ChunkColumn;
            }
        }

        public bool IsChunkLoaded(Index2 position)
            => cache.ContainsKey(new Index3(position, Planet.Id));

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            _autoResetEvent.Set();
            ChunkColumnChanged?.Invoke(this, chunkColumn);
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(Index2 position)
        {
            if (cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }


        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            using (semaphore.Wait())
            {
                foreach (CacheItem value in cache.Values)
                {
                    value.References = 0;
                    _unreferencedItems.Enqueue(value);
                }
            }
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(Index2 position)
        {
            using (semaphore.Wait())
            {
                if (!cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                {
                    throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                }

                if (--cacheItem.References <= 0)
                {
                    if (cacheItem.References < 0)
                        logger.Warn($"Remove Reference from {cacheItem.Index}, now at: {cacheItem.References}");

                    _unreferencedItems.Enqueue(cacheItem);
                    _autoResetEvent.Set();
                }
            }
        }

        private Task BackgroundCleanup(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                _autoResetEvent.WaitOne();

                while (_unreferencedItems.TryDequeue(out CacheItem ci))
                {
                    if (ci.References <= 0)
                    {
                        var key = new Index3(ci.Index, ci.Planet.Id);

                        using (ci.Wait())
                            ci.Changed -= ItemChanged;

                        using (semaphore.Wait())
                            cache.Remove(key);

                        using (updateSemaphore.Wait())
                            oldChunks.Enqueue(ci);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (updateSemaphore)
            {
                //Neue Chunks in die Simulation einpflegen
                while (newChunks.Count > 0)
                {
                    CacheItem chunk = newChunks.Dequeue();
                    chunk.ChunkColumn.ForEachEntity(simulation.Add);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (oldChunks.Count > 0)
                {
                    using (CacheItem chunk = oldChunks.Dequeue())
                    {
                        chunk.ChunkColumn.ForEachEntity(simulation.Remove);
                    }
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            //using (semaphore.Wait())
            //{
            //    FailEntityChunkArgs[] failChunkEntities = cache
            //        .Where(chunk => chunk.Value.ChunkColumn != null)
            //        .SelectMany(chunk => chunk.Value.ChunkColumn.FailChunkEntity())
            //        .ToArray();

            //    foreach (FailEntityChunkArgs entity in failChunkEntities)
            //    {
            //        IChunkColumn currentchunk = Peek(entity.CurrentChunk);
            //        IChunkColumn targetchunk = Peek(entity.TargetChunk);

            //        currentchunk?.Remove(entity.Entity);

            //        if (targetchunk != null)
            //        {
            //            targetchunk.Add(entity.Entity);
            //        }
            //        else
            //        {
            //            targetchunk = resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);

            //            simulation.RemoveEntity(entity.Entity); //Because we add it again through the targetchunk
            //            targetchunk.Add(entity.Entity);
            //        }
            //    }
            //}
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
            if (notification is IChunkNotification chunk
                && cache.TryGetValue(new Index3(chunk.ChunkPos.X, chunk.ChunkPos.Y, chunk.Planet),
                out CacheItem cacheItem))
            {
                cacheItem.ChunkColumn?.Update(notification);
            }
        }

        public void Dispose()
        {
            foreach (var item in _unreferencedItems.ToArray())
                item.Dispose();

            foreach (var item in cache.ToArray())
                item.Value.Dispose();

            foreach (var item in newChunks.ToArray())
                item.Dispose();

            foreach (var item in oldChunks.ToArray())
                item.Dispose();


            cache.Clear();
            newChunks.Clear();
            oldChunks.Clear();

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

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem : IDisposable
        {

            private ChunkPool chunkPool;
            private IChunkColumn _chunkColumn;
            private readonly LockSemaphore internalSemaphore;

            public IPlanet Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }


            /// <summary>
            /// Der Chunk, auf den das <see cref="CacheItem"/> referenziert
            /// </summary>
            public IChunkColumn ChunkColumn
            {
                get => _chunkColumn;
                set
                {
                    if (_chunkColumn != null)
                        _chunkColumn.Changed -= OnChanged;

                    _chunkColumn = value;

                    if (value != null)
                        value.Changed += OnChanged;
                }
            }

            public event Action<CacheItem, IChunkColumn> Changed;

            private bool disposed;

            public CacheItem(ChunkPool chunkPool)
            {
                internalSemaphore = new LockSemaphore(1, 1);

                this.chunkPool = chunkPool;
            }

            public LockSemaphore.SemaphoreLock Wait()
                => internalSemaphore.Wait();

            public void Dispose()
            {
                if (disposed)
                    return;

                disposed = true;

                internalSemaphore.Dispose();

                foreach (var chunk in _chunkColumn.Chunks)
                {
                    chunkPool.Push(chunk);
                }

                if (_chunkColumn is IDisposable disposable)
                    disposable.Dispose();

                _chunkColumn = null;
                Planet = null;
            }

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk)
                => Changed?.Invoke(this, chunkColumn);

        }

    }
}
