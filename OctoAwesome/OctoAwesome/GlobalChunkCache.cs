using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Funktion, die für das Laden der Chunks verwendet wird
        /// </summary>
        private Func<int, Index2, IChunkColumn> loadDelegate;

        /// <summary>
        /// Routine, die für das Speichern der Chunks verwendet wird.
        /// </summary>
        private Action<int, Index2, IChunkColumn> saveDelegate;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns { get { return cache.Count; } }

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse GlobalChunkCache
        /// </summary>
        /// <param name="loadDelegate">Delegat, der nicht geladene ChunkColumns nachläd.</param>
        /// <param name="saveDelegate">Delegat, der nicht mehr benötigte ChunkColumns abspeichert.</param>
        public GlobalChunkCache(Func<int, Index2, IChunkColumn> loadDelegate,
            Action<int, Index2, IChunkColumn> saveDelegate)
        {
            if (loadDelegate == null) throw new ArgumentNullException("loadDelegate");
            if (saveDelegate == null) throw new ArgumentNullException("saveDelegate");

            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;

            cache = new Dictionary<Index3, CacheItem>();
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <param name="writeable">Gibt an, ob der Subscriber schreibend zugreifen will</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(int planet, Index2 position, bool writeable)
        {
            CacheItem cacheItem = null;

            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    cacheItem = new CacheItem()
                    {
                        References = 0,
                        ChunkColumn = null,
                    };

                    cache.Add(new Index3(position, planet), cacheItem);
                }

                cacheItem.References++;
            }

            lock (cacheItem)
            {
                if (cacheItem.ChunkColumn == null)
                {
                    cacheItem.ChunkColumn = loadDelegate(planet, position);
                    cacheItem.SavedChangeCounter = cacheItem.ChunkColumn.Chunks.Select(c => c.ChangeCounter).ToArray();
                }
            }

            return cacheItem.ChunkColumn;
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
                foreach (var item in cache.Values)
                {
                    saveDelegate(
                        item.ChunkColumn.Planet,
                        item.ChunkColumn.Index,
                        item.ChunkColumn);

                    item.ChunkColumn = null;
                }

                cache.Clear();
            }
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        /// <param name="writeable">Ist der Chunk schreibbar abonniert worden?</param>
        public void Release(int planet, Index2 position, bool writeable)
        {
            CacheItem cacheItem = null;
            lock (lockObject)
            {
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    throw new NotSupportedException("Kein Chunk für Position in Cache");
                }

                cacheItem.References--;
            }

            //lock (cacheItem)
            //{
            //    if (cacheItem.References <= 0 && cacheItem.ChunkColumn != null)
            //    {
            //        saveDelegate(planet, position, cacheItem.ChunkColumn);
            //    }
            //}

            //lock (lockObject)
            //{
            //    if (cacheItem.References <= 0)
            //    {
            //        cacheItem.ChunkColumn = null;
            //        cache.Remove(new Index3(position, planet));
            //    }
            //}
        }

        private void Cleanup()
        {
            // Items mit ChangeCounter sichern
            CacheItem[] cacheItems;
            lock (lockObject)
            {
                cacheItems = cache.Values.Where(v => v.IsDirty()).ToArray();
            }

            foreach (var cacheItem in cacheItems)
            {
                lock (cacheItem)
                {
                    cacheItem.SavedChangeCounter = cacheItem.ChunkColumn.Chunks.Select(c => c.ChangeCounter).ToArray();
                    saveDelegate(cacheItem.Planet, cacheItem.Index, cacheItem.ChunkColumn);
                }
            }
            
            // Items ohne Ref aus Cache entfernen
            lock (lockObject)
            {
                var keys = cache.Where(v => v.Value.References == 0 && v.Value.ChunkColumn != null && !v.Value.IsDirty()).Select(v => v.Key).ToArray();
                foreach (var key in keys)
                {
                    cache[key].ChunkColumn = null;
                    cache.Remove(key);
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
                    if (ChunkColumn.Chunks[i].ChangeCounter > SavedChangeCounter[i])
                        return true;
                }

                return false;
            }
        }
    }
}
