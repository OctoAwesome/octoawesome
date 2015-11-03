using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Chunk Cache für lokale Anwendungen.
    /// </summary>
    public sealed class LocalChunkCache : ILocalChunkCache
    {
        private IGlobalChunkCache globalCache;

        private readonly IChunk[][] chunks;

        private IPlanet planet;
        private int limit;
        private int mask;
        private int range;

        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="globalCache">Referenz auf global Chunk Cache</param>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        /// <param name="range">Gibt die Range in alle Richtungen an.</param>
        public LocalChunkCache(IGlobalChunkCache globalCache, int dimensions, int range)
        {
            if (1 << dimensions < (range * 2) + 1)
                throw new ArgumentException("Range too big");

            this.globalCache = globalCache;
            this.range = range;

            limit = dimensions;
            mask = (1 << limit) - 1;
            chunks = new IChunk[(mask + 1) * (mask + 1)][];
        }

        public void SetCenter(IPlanet planet, Index3 index)
        {
            // Planet resetten falls notwendig
            if (this.planet != planet)
                InitializePlanet(planet);

            if (planet == null) return;

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int z = 0; z < planet.Size.Z; z++)
                    {
                        Index3 local = new Index3(
                            index.X + x,
                            index.Y + y,
                            z);

                        local.NormalizeXY(planet.Size);

                        int localX = local.X & mask;
                        int localY = local.Y & mask;

                        IChunk chunk = chunks[FlatIndex(localX, localY)][z];

                        // Alten Chunk entfernen, falls notwendig
                        if (chunk != null && chunk.Index != local)
                        {
                            globalCache.Release(new PlanetIndex3(planet.Id, local));
                            chunks[FlatIndex(localX, localY)][z] = null;
                            chunk = null;
                        }

                        // Neuen Chunk laden
                        if (chunk == null)
                        {
                            chunk = globalCache.Subscribe(new PlanetIndex3(planet.Id, local));
                            chunks[FlatIndex(localX, localY)][z] = chunk;
                        }
                    }
                }
            }
        }

        private void InitializePlanet(IPlanet planet)
        {
            if (this.planet != null)
                Flush();

            this.planet = planet;
            int height = planet.Size.Z;

            for (int i = 0; i < chunks.Length; i++)
                chunks[i] = new IChunk[height];
        }

        [Obsolete]
        public IChunk GetChunk(Index3 index)
        {
            return GetChunk(index.X, index.Y, index.Z);
        }

        public IChunk GetChunk(int x, int y, int z)
        {
            if (planet == null) return null;
            if (z < 0) return null;
            if (z >= planet.Size.Z) return null;

            IChunk chunk = chunks[FlatIndex(x, y)][z];
            if (chunk != null && chunk.Index.X == x && chunk.Index.Y == y && chunk.Index.Z == z)
                return chunk;
            return null;
        }

        [Obsolete]
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public ushort GetBlock(int x, int y, int z)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            if (chunk != null)
                return chunk.GetBlock(x, y, z);

            return 0;
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        [Obsolete]
        public void SetBlock(Index3 index, ushort block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, ushort block)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            if (chunk != null)
                chunk.SetBlock(x, y, z, block);
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            if (chunk != null)
                return chunk.GetBlockMeta(x, y, z);

            return 0;
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            IChunk chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            if (chunk != null)
                chunk.SetBlockMeta(x, y, z, meta);
        }

        public void Flush()
        {
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i] == null) continue;
                for (int h = 0; h < chunks[i].Length; h++)
                {
                    IChunk chunk = chunks[i][h];
                    if (chunk != null)
                    {
                        globalCache.Release(new PlanetIndex3(chunk.Planet, chunk.Index));
                        chunks[i][h] = null;
                    }
                }
            }
        }

        private int FlatIndex(int x, int y)
        {
            return (((y & (mask)) << limit) | ((x & (mask))));
        }
    }
}
