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
        /// <param name="writeable">Gibt an, ob der Subscriber schreibend zugreifen will</param>
        /// <returns></returns>
        public IChunk Subscribe(PlanetIndex3 position, bool writeable)
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
                if (writeable) cacheItem.WritableReferences++;
                return cacheItem.Chunk;
            }
        }

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="position"></param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        public IChunk GetChunk(PlanetIndex3 position)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (cache.TryGetValue(position, out cacheItem))
                    return cacheItem.Chunk;
                return null;
            }
        }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="writeable"></param>
        public void Release(PlanetIndex3 position, bool writeable)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(position, out cacheItem))
                {
                    throw new NotSupportedException("Kein Chunk für Position in Cache");
                }

                cacheItem.References--;
                if (writeable) cacheItem.WritableReferences--;

                if (cacheItem.WritableReferences <= 0)
                {
                    saveDelegate(position, cacheItem.Chunk);
                }

                if (cacheItem.References <= 0)
                {
                    cacheItem.Chunk = null;
                    cache.Remove(position);
                }
            }
        }

        private class CacheItem
        {
            public PlanetIndex3 Position { get; set; }

            public int References { get; set; }

            public int WritableReferences { get; set; }

            public IChunk Chunk { get; set; }
        }
    }
}
