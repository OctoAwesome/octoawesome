using OctoAwesome.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Globaler Cache für Chunks
    /// </summary>
    public sealed class GlobalChunkCache : IGlobalChunkCache
    {

        public event EventHandler<IChunkColumn> ChunkColumnChanged;

        private readonly ConcurrentQueue<CacheItem> _dirtyItems = new ConcurrentQueue<CacheItem>();
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
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim updateSemaphore = new SemaphoreSlim(1, 1);

        // TODO: Früher oder später nach draußen auslagern
        private readonly Task cleanupTask;
        private IUpdateHub updateHub;


        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                using (new CacheLock(semaphore))
                {
                    return cache.Count;
                }
            }
        }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => _dirtyItems.Count;

        public IPlanet Planet { get; }

        /// <summary>
        /// Create new instance of GlobalChunkCache
        /// </summary>
        /// <param name="resourceManager">the current <see cref="IResourceManager"/> to load ressources/></param>
        public GlobalChunkCache(IPlanet planet, IResourceManager resourceManager)
        {
            Planet = planet ?? throw new ArgumentNullException(nameof(planet));
            this.resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));

            cache = new Dictionary<Index3, CacheItem>();
            newChunks = new Queue<CacheItem>();
            oldChunks = new Queue<CacheItem>();

            tokenSource = new CancellationTokenSource();
            cleanupTask = new Task(async () => await BackgroundCleanup(tokenSource.Token), TaskCreationOptions.LongRunning);
            cleanupTask.Start(TaskScheduler.Default);
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

            semaphore.Wait();

            if (!cache.TryGetValue(new Index3(position, Planet.Id), out cacheItem))
            {

                cacheItem = new CacheItem()
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
            semaphore.Release();


            if (cacheItem.ChunkColumn == null)
            {
                cacheItem.Wait();
                cacheItem.ChunkColumn = resourceManager.LoadChunkColumn(Planet, position);
                updateSemaphore.Wait();
                newChunks.Enqueue(cacheItem);
                updateSemaphore.Release();
                cacheItem.Release();
            }

            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(Index2 position)
            => cache.ContainsKey(new Index3(position, Planet.Id));

        private void ItemChanged(CacheItem obj, IChunkColumn chunkColumn)
        {
            _dirtyItems.Enqueue(obj);
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
            semaphore.Wait();
            foreach (var value in cache.Values)
            {
                value.References = 0;
                _unreferencedItems.Enqueue(value);
            }
            semaphore.Release();
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(Index2 position)
        {
            using (new CacheLock(semaphore))
            {
                if (!cache.TryGetValue(new Index3(position, Planet.Id), out CacheItem cacheItem))
                {
                        throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));                    
                }

                    if (--cacheItem.References <= 0)
                    {
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
                CacheItem ci;
                var itemsToSave = new List<CacheItem>();

                while (_dirtyItems.TryDequeue(out ci))
                    itemsToSave.Add(ci);

                foreach (var item in itemsToSave.Distinct())
                {
                    lock (item)
                    {
                        resourceManager.SaveChunkColumn(item.ChunkColumn);
                    }
                }

                while (_unreferencedItems.TryDequeue(out ci))
                {
                    if (ci.References <= 0)
                    {
                        var key = new Index3(ci.Index, ci.Planet.Id);

                        using (ci.Lock())
                            ci.Changed -= ItemChanged;

                        using (new CacheLock(semaphore))
                            cache.Remove(key);

                        using (new CacheLock(updateSemaphore))
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
                    var chunk = newChunks.Dequeue();

                    foreach (var entity in chunk.ChunkColumn.Entities)
                        simulation.AddEntity(entity);
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (oldChunks.Count > 0)
                {
                    var chunk = oldChunks.Dequeue();

                    foreach (var entity in chunk.ChunkColumn.Entities)
                        simulation.RemoveEntity(entity);
                    chunk.Dispose();
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            using (new CacheLock(semaphore))
            {
                var failChunkEntities = cache
                    .Where(chunk => chunk.Value.ChunkColumn != null)
                    .SelectMany(chunk => chunk.Value.ChunkColumn.Entities.FailChunkEntity())
                    .ToArray();

                foreach (var entity in failChunkEntities)
                {
                    //TODO: Old Planet change
                    //var currentchunk = Peek(entity.CurrentPlanet, entity.CurrentChunk);
                    //var targetchunk = Peek(entity.TargetPlanet, entity.TargetChunk);
                    var currentchunk = Peek(entity.CurrentChunk);
                    var targetchunk = Peek(entity.TargetChunk);

                    currentchunk.Entities.Remove(entity.Entity);

                    if (targetchunk != null)
                    {
                        targetchunk.Entities.Add(entity.Entity);
                    }
                    else
                    {
                        targetchunk = resourceManager.LoadChunkColumn(entity.CurrentPlanet, entity.TargetChunk);
                        targetchunk.Entities.Add(entity.Entity);
                        resourceManager.SaveChunkColumn(targetchunk);
                        simulation.RemoveEntity(entity.Entity);
                    }
                }

            }
        }

        public void OnCompleted() { }

        public void OnError(Exception error)
            => throw error;

        public void OnNext(Notification value)
        {
            switch (value)
            {
                case ChunkNotification chunkNotification:
                    Update(chunkNotification);
                    break;
                default:
                    break;
            }
        }

        public void OnUpdate(SerializableNotification notification)
            => updateHub?.Push(notification, DefaultChannels.Network);

        public void Update(SerializableNotification notification)
        {
            if (notification is ChunkNotification chunkNotification &&
                cache.TryGetValue(new Index3(chunkNotification.ChunkColumnIndex, chunkNotification.Planet),
                out var cacheItem))
            {
                cacheItem.ChunkColumn.Update(notification);
            }
        }

        public void InsertUpdateHub(IUpdateHub updateHub)
            => this.updateHub = updateHub;

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem : IDisposable
        {
            private IChunkColumn _chunkColumn;
            private readonly SemaphoreSlim internalSemaphore;

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

            public CacheItem()
            {
                internalSemaphore = new SemaphoreSlim(1, 1);
            }

            public void Wait()
            {
                internalSemaphore?.Wait();
            }

            public void Release()
            {
                internalSemaphore.Release();
            }

            public CacheItemLock Lock()
                => new CacheItemLock(this);

            public void Dispose()
            {
                internalSemaphore.Dispose();
            }

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk, int changeCounter)
                => Changed?.Invoke(this, chunkColumn);

            public struct CacheItemLock : IDisposable
            {
                private readonly CacheItem cacheItem;

                public CacheItemLock(CacheItem cacheItem)
                {
                    this.cacheItem = cacheItem;
                    cacheItem.Wait();
                }

                public void Dispose()
                {
                    cacheItem.Release();
                }
            }
        }

        public struct CacheLock : IDisposable
        {
            private readonly SemaphoreSlim internalSemaphore;

            public CacheLock(SemaphoreSlim semaphoreSlim)
            {
                internalSemaphore = semaphoreSlim;
                internalSemaphore.Wait();
            }

            public void Dispose()
            {
                internalSemaphore.Release();
            }
        }
    }
}
