using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class GlobalChunkCache
    {
        private Dictionary<PlanetIndex3, CacheItem> cache;

        private Func<PlanetIndex3, IChunk> loadDelegate;

        private Action<PlanetIndex3, IChunk> saveDelegate;

        private object lockObject = new object();

        public GlobalChunkCache(Func<PlanetIndex3, IChunk> loadDelegate, 
            Action<PlanetIndex3, IChunk> saveDelegate)
        {
            if (loadDelegate == null) throw new ArgumentNullException("loadDelegate");
            if (saveDelegate == null) throw new ArgumentNullException("saveDelegate");

            this.loadDelegate = loadDelegate;
            this.saveDelegate = saveDelegate;

            cache = new Dictionary<PlanetIndex3, CacheItem>();
        }

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

        public void Release(PlanetIndex3 position)
        {
            lock (lockObject)
            {
                CacheItem cacheItem = null;
                if (!cache.TryGetValue(position, out cacheItem))
                {
                    throw new Exception("Kein Chunk für Position in Cache");
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
