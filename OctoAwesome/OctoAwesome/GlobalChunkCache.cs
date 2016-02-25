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
        private Dictionary<PlanetIndex3, CacheItem> cache;

        /// <summary>
        /// Funktion, die für das Laden der Chunks verwendet wird
        /// </summary>
        private Func<PlanetIndex3, IChunk> loadDelegate;

        /// <summary>
        /// Routine, die für das Speichern der Chunks verwendet wird.
        /// </summary>
        private Action<PlanetIndex3, IChunk> saveDelegate;

        /// <summary>
        /// Objekt, das für die Locks benutzt wird
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// Gibt die Anzahl der aktuell geladenen Chunks zurück.
        /// </summary>
        public int LoadedChunks
        {
            get { return cache.Count; }
        }

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse GlobalChunkCache
        /// </summary>
        /// <param name="loadDelegate">Delegat, der nicht geladene Chubks nachläd.</param>
        /// <param name="saveDelegate">Delegat, der nicht mehr benötigte Chunks abspeichert.</param>
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
        /// <returns>Den neu abonnierten Chunk</returns>
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
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
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
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        /// <param name="writeable">Ist der Chunk schreibbar abonniert worden?</param>
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

        /// <summary>
        /// Element für den Cache
        /// </summary>
        private class CacheItem
        {
            /// <summary>
            /// Die Position des <see cref="CacheItem"/> in der Welt
            /// </summary>
            public PlanetIndex3 Position { get; set; }

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
            public IChunk Chunk { get; set; }
        }
    }
}