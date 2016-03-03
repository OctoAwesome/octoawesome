using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private Action<int,Index2, IChunkColumn> saveDelegate;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunkColumns { get { return cache.Count; } }

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
        /// <param name="position">Position des Chunks</param>
        /// <param name="writeable">Gibt an, ob der Subscriber schreibend zugreifen will</param>
        /// <returns></returns>
        public IChunkColumn Subscribe(int planet,Index2 position, bool writeable)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(new Index3(position, planet), out cacheItem))
                {
                    cacheItem = new CacheItem()
                    {
                        References = 0,
                        ChunkColumn = loadDelegate(planet,position),
                    };
                    
                    cache.Add(new Index3(position, planet), cacheItem);
                }
                cacheItem.References++;
                if (writeable) cacheItem.WritableReferences++;
                return cacheItem.ChunkColumn;
            }
        }
        public IChunkColumn Peek(int planet, Index2 position)
        {
            CacheItem cacheItem = null;
            if (cache.TryGetValue(new Index3(position, planet),out cacheItem))
            {
                return cacheItem.ChunkColumn;
            }
            return null;
        }

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
        /// <param name="position">Die Position des Chunks der Freigegeben wird</param>
        /// <param name="writeable">Gibt an, ob der Subscriber schreibend zugegriffen hat</param>
        public void Release(int planet,Index2 position, bool writeable)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(new Index3(position,planet), out cacheItem))
                {
                    throw new NotSupportedException("Kein Chunk für Position in Cache");
                }

                cacheItem.References--;
                if (writeable) cacheItem.WritableReferences--;

                if (cacheItem.WritableReferences <= 0)
                {
                    saveDelegate(planet,position, cacheItem.ChunkColumn);
                }

                if (cacheItem.References <= 0)
                {
                    cacheItem.ChunkColumn = null;
                    cache.Remove(new Index3(position,planet));
                }
            }
        }

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem
        {
            
            /// <summary>
            /// Die Zahl der Subscriber, die das Item Abboniert hat.
            /// </summary>
            public int References { get; set; }

            /// <summary>
            /// Die Zahl der Subscriber, die schreibend auf den Chunk zugreifen. Ihre Referenz wird auch in <see cref="References"/> mitgezählt
            /// </summary>
            public int WritableReferences { get; set; }

            /// <summary>
            /// Der Chunk, auf den das <see cref="CacheItem"/> referenziert
            /// </summary>
            public IChunkColumn ChunkColumn { get; set; }
        }
    }
}
