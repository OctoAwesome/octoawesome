using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
        private Queue<CacheItem> newChunks;
        private Queue<CacheItem> oldChunks;

        private readonly object updateLockObject = new object();

        /// <summary>
        /// Funktion, die für das Laden der Chunks verwendet wird
        /// </summary>
        private readonly Func<int, Index2, IChunkColumn> loadDelegate;

        /// <summary>
        /// Routine, die für das Speichern der Chunks verwendet wird.
        /// </summary>
        private readonly Action<int, Index2, IChunkColumn> saveDelegate;

        private readonly Func<int, IPlanet> loadPlanetDelagte;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private readonly object lockObject = new object();

        // TODO: Früher oder später nach draußen auslagern
        private Thread cleanupThread;


        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns
        {
            get
            {
                lock (lockObject)
                {
                    return cache.Count;
                }
            }
        }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn => _dirtyItems.Count;

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse GlobalChunkCache
        /// </summary>
        /// <param name="loadDelegate">Delegat, der nicht geladene ChunkColumns nachläd.</param>
        /// <param name="loadPlanetDelegate"></param>
        /// <param name="saveDelegate">Delegat, der nicht mehr benötigte ChunkColumns abspeichert.</param>
        public GlobalChunkCache(Func<int, Index2, IChunkColumn> loadDelegate, Func<int, IPlanet> loadPlanetDelegate,
            Action<int, Index2, IChunkColumn> saveDelegate)
        {
            this.loadDelegate = loadDelegate ?? throw new ArgumentNullException("loadDelegate");
            this.saveDelegate = saveDelegate ?? throw new ArgumentNullException("saveDelegate");
            loadPlanetDelagte = loadPlanetDelegate ?? throw new ArgumentNullException(nameof(loadPlanetDelegate));

            cache = new Dictionary<Index3, CacheItem>();
            newChunks = new Queue<CacheItem>();
            oldChunks = new Queue<CacheItem>();

            cleanupThread = new Thread(BackgroundCleanup)
            {
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            cleanupThread.Start();
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(int planet, Index2 position, bool passive)
        {
            CacheItem cacheItem = null;

            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    //TODO: Überdenken
                    if (passive)
                        return null;

                    cacheItem = new CacheItem()
                    {
                        Planet = planet,
                        Index = position,
                        References = 0,
                        PassiveReference = 0,
                        ChunkColumn = null,
                    };

                    cacheItem.Changed += ItemChanged;
                    //_dirtyItems.Enqueue(cacheItem);
                    cache.Add(new Index3(position, planet), cacheItem);
                    //_autoResetEvent.Set();
                }

                if (passive)
                    cacheItem.PassiveReference++;
                else
                    cacheItem.References++;
            }

            lock (cacheItem)
            {
                if (cacheItem.ChunkColumn == null)
                {
                    cacheItem.ChunkColumn = loadDelegate(planet, position);

                    lock (updateLockObject)
                    {
                        newChunks.Enqueue(cacheItem);
                    }
                }
            }

            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(int planet, Index2 position)
            => cache.ContainsKey(new Index3(position, planet));

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
        public IChunkColumn Peek(int planet, Index2 position)
        {
            if (cache.TryGetValue(new Index3(position, planet), out CacheItem cacheItem))
                return cacheItem.ChunkColumn;

            return null;
        }


        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        public void Clear()
        {
            lock (lockObject)
            {
                foreach (var value in cache.Values)
                {
                    value.References = 0;
                    value.PassiveReference = 0;
                    _unreferencedItems.Enqueue(value);
                }
            }
            _autoResetEvent.Set();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(int planet, Index2 position, bool passive)
        {
            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out CacheItem cacheItem))
                {
                    if (!passive)
                    {
                        throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                    }

                    return;
                }

                if (passive)
                {
                    if (cacheItem != null)
                        cacheItem.PassiveReference--;
                }
                else
                {
                    if (--cacheItem.References <= 0)
                    {
                        _unreferencedItems.Enqueue(cacheItem);
                        _autoResetEvent.Set();
                    }
                }
            }
        }

        private void BackgroundCleanup()
        {
            while (true)
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
                        saveDelegate(item.Planet, item.Index, item.ChunkColumn);
                    }
                }

                while (_unreferencedItems.TryDequeue(out ci))
                {
                    lock (ci)
                    {
                        if (ci.References <= 0)
                        {
                            var key = new Index3(ci.Index, ci.Planet);

                            lock (lockObject)
                            {
                                ci.Changed -= ItemChanged;
                                cache.Remove(key);
                            }
                            lock (updateLockObject)
                            {
                                oldChunks.Enqueue(ci);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gibt einen Planenten anhand seiner ID zurück
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <returns>Planet</returns>
        public IPlanet GetPlanet(int id)
            => loadPlanetDelagte(id);

        public void BeforeSimulationUpdate(Simulation simulation)
        {
            lock (updateLockObject)
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
                }
            }
        }

        public void AfterSimulationUpdate(Simulation simulation)
        {
            //TODO: Überarbeiten
            lock (lockObject)
            {
                var failChunkEntities = cache
                    .Where(chunk => chunk.Value.ChunkColumn != null)
                    .SelectMany(chunk => chunk.Value.ChunkColumn.Entities.FailChunkEntity())
                    .ToArray();

                foreach (var entity in failChunkEntities)
                {
                    var currentchunk = Peek(entity.CurrentPlanet, entity.CurrentChunk);
                    var targetchunk = Peek(entity.TargetPlanet, entity.TargetChunk);

                    currentchunk.Entities.Remove(entity.Entity);

                    if (targetchunk != null)
                    {
                        targetchunk.Entities.Add(entity.Entity);
                    }
                    else
                    {
                        targetchunk = loadDelegate(entity.CurrentPlanet, entity.TargetChunk);
                        targetchunk.Entities.Add(entity.Entity);
                        saveDelegate(entity.CurrentPlanet, entity.TargetChunk, targetchunk);
                        simulation.RemoveEntity(entity.Entity);
                    }
                }

            }
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem
        {
            private IChunkColumn _chunkColumn;
            public int Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }

            public int PassiveReference { get; set; }

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

            private void OnChanged(IChunkColumn chunkColumn, IChunk chunk, int changeCounter)
                => Changed?.Invoke(this, chunkColumn);
        }
    }
}
