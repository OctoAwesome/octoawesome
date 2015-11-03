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
        private Dictionary<PlanetIndex3, CacheItem> cache;

        private Func<PlanetIndex3, IChunk> loadDelegate;

        private Action<PlanetIndex3, IChunk> saveDelegate;

        private object lockObject = new object();

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunks { get { return cache.Count; } }

        public GlobalChunkCache(Func<PlanetIndex3, IChunk> loadDelegate, 
            Action<PlanetIndex3, IChunk> saveDelegate)
        {
            if (loadDelegate == null) throw new ArgumentNullException("loadDelegate");
            if (saveDelegate == null) throw new ArgumentNullException("saveDelegate");

            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;

            cache = new Dictionary<PlanetIndex3, CacheItem>();
        }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        public IChunk Subscribe(PlanetIndex3 position)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(position, out cacheItem))
                {
                    cacheItem = new CacheItem()
                    {
                        Position = position,
                        References = 0,
                        Chunk = loadDelegate(position),
                    };

                    cache.Add(position, cacheItem);
                }
                cacheItem.References++;
                return cacheItem.Chunk;
            }
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position"></param>
        public void Release(PlanetIndex3 position)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(position, out cacheItem))
                {
                    throw new NotSupportedException("Kein Chunk für Position in Cache");
                }

                cacheItem.References--;
                if (cacheItem.References <= 0)
                {
                    saveDelegate(position, cacheItem.Chunk);
                    cacheItem.Chunk = null;
                    cache.Remove(position);
                }
            }
        }

        private class CacheItem
        {
            public PlanetIndex3 Position { get; set; }

            public int References { get; set; }

            public IChunk Chunk { get; set; }
        }
    }
}
