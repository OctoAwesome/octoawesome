using System;
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
        /// <summary>
        /// Dictionary, das alle <see cref="CacheItem"/>s hält.
        /// </summary>
        private Dictionary<Index3, CacheItem> cache;

        private Queue<CacheItem> newchunks;

        private Queue<CacheItem> oldchunks;

        private object updatelockobject = new object();

        /// <summary>
        /// Funktion, die für das Laden der Chunks verwendet wird
        /// </summary>
        private Func<int, Index2, IChunkColumn> loadDelegate;

        /// <summary>
        /// Routine, die für das Speichern der Chunks verwendet wird.
        /// </summary>
        private Action<int, Index2, IChunkColumn> saveDelegate;

        private Func<int, IPlanet> loadPlanetDelagte;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private object lockObject = new object();

        // TODO: Früher oder später nach draußen auslagern
        private Thread cleanupThread;

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns { get { return cache.Count; } }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        public int DirtyChunkColumn
        {
            get
            {
                lock (lockObject)
                {
                    return cache.Values.Where(v => v.IsDirty()).Count();
                }
            }
        }

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse GlobalChunkCache
        /// </summary>
        /// <param name="loadDelegate">Delegat, der nicht geladene ChunkColumns nachläd.</param>
        /// <param name="saveDelegate">Delegat, der nicht mehr benötigte ChunkColumns abspeichert.</param>
        public GlobalChunkCache(Func<int, Index2, IChunkColumn> loadDelegate,Func<int,IPlanet> loadPlanetDelegate,
            Action<int, Index2, IChunkColumn> saveDelegate)
        {
            if (loadDelegate == null) throw new ArgumentNullException("loadDelegate");
            if (saveDelegate == null) throw new ArgumentNullException("saveDelegate");
            if (loadPlanetDelegate == null) throw new ArgumentNullException(nameof(loadPlanetDelegate));

            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;
            this.loadPlanetDelagte = loadPlanetDelegate;

            cache = new Dictionary<Index3, CacheItem>();
            newchunks = new Queue<CacheItem>();
            oldchunks = new Queue<CacheItem>();

            cleanupThread = new Thread(BackgroundCleanup);
            cleanupThread.IsBackground = true;
            cleanupThread.Priority = ThreadPriority.Lowest;
            cleanupThread.Start();
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(int planet, Index2 position,bool passive)
        {
            CacheItem cacheItem = null;

            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    //TODO: Überdenken
                    if (passive)
                    {
                        return null;
                    }

                    cacheItem = new CacheItem()
                    {
                        Planet = planet,
                        Index = position,
                        References = 0,
                        PassiveReference = 0,
                        ChunkColumn = null,
                    };

                    cache.Add(new Index3(position, planet), cacheItem);
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
                    cacheItem.SavedChangeCounter = cacheItem.ChunkColumn.Chunks.Select(c => c.ChangeCounter).ToArray();

                    lock (updatelockobject)
                    {
                        newchunks.Enqueue(cacheItem);
                    }
                }
            }

            return cacheItem.ChunkColumn;
        }

        public bool IsChunkLoaded(int planet, Index2 position)
        {
            return cache.ContainsKey(new Index3(position, planet));
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunkColumn Peek(int planet, Index2 position)
        {
            CacheItem cacheItem = null;
            if (cache.TryGetValue(new Index3(position, planet), out cacheItem))
            {
                return cacheItem.ChunkColumn;
            }
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
                }
            }

            Cleanup();
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        public void Release(int planet, Index2 position,bool passive)
        {
            CacheItem cacheItem = null;
            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    if (!passive)
                    {
                        throw new NotSupportedException(string.Format("Kein Chunk für die Position ({0}) im Cache", position));
                    }
                    
                }

                if (passive)
                {
                    if (cacheItem != null)
                        cacheItem.PassiveReference--;
                }
                else
                    cacheItem.References--;
            }
        }

        private void BackgroundCleanup()
        {
            while (true)
            {
                Cleanup();
                Thread.Sleep(100);
            }
        }

        private void Cleanup()
        {
            // Items mit ChangeCounter sichern
            CacheItem[] cacheItems;
            lock (lockObject)
            {
                cacheItems = cache.Values.Where(v => v.IsDirty()).ToArray();
                // DirtyChunkColumn = cacheItems.Length;
            }

            foreach (var cacheItem in cacheItems)
            {
                lock (cacheItem)
                {
                    if (cacheItem.IsDirty())
                    {
                        cacheItem.SavedChangeCounter = cacheItem.ChunkColumn.Chunks.Select(c => c.ChangeCounter).ToArray();
                        saveDelegate(cacheItem.Planet, cacheItem.Index, cacheItem.ChunkColumn);
                    }
                }
            }

            // Items ohne Ref aus Cache entfernen
            lock (lockObject)
            {
                var keys = cache.Where(v => v.Value.References == 0 && v.Value.ChunkColumn != null && !v.Value.IsDirty()).ToArray();
                foreach (var key in keys)
                {
                    // cache[key].ChunkColumn = null;
                    cache.Remove(key.Key);

                    lock (updatelockobject)
                    {
                        oldchunks.Enqueue(key.Value);
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
        {
            return loadPlanetDelagte(id);
        }

        public void SimulationUpdate(Simulation simulation)
        {
            lock (updatelockobject)
            {
                //Neue Chunks in die Simulation einpflegen
                while (newchunks.Count > 0)
                {
                    var chunk = newchunks.Dequeue();
                    foreach (var entity in chunk.ChunkColumn.Entities)
                    {
                        simulation.AddEntity(entity);
                    }
                }

                //Alte Chunks aus der Siumaltion entfernen
                while (oldchunks.Count > 0)
                {
                    var chunk = oldchunks.Dequeue();
                    foreach (var entity in chunk.ChunkColumn.Entities)
                    {
                        simulation.RemoveEntity(entity);
                    }
                }
            }
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem
        {
            public int Planet { get; set; }

            public Index2 Index { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }

            public int PassiveReference { get; set; }

            public int[] SavedChangeCounter { get; set; }

            /// <summary>
            /// Der Chunk, auf den das <see cref="CacheItem"/> referenziert
            /// </summary>
            public IChunkColumn ChunkColumn { get; set; }

            public bool IsDirty()
            {
                if (ChunkColumn == null) return false;

                for (int i = 0; i < ChunkColumn.Chunks.Length; i++)
                {
                    if (ChunkColumn.Chunks[i] != null && ChunkColumn.Chunks[i].ChangeCounter != SavedChangeCounter[i])
                        return true;
                }

                return false;
            }
        }
    }
}
