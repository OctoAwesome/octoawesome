using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Chunk Cache für lokale Anwendungen.
    /// </summary>
    public sealed class LocalChunkCache
    {
        private IGlobalChunkCache globalCache;

        private readonly IChunk[][] chunks;

        private IPlanet planet;
        private int limitX;
        private int limitY;
        private int maskX;
        private int maskY;
        private int range;

        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="globalCache">Referenz auf global Chunk Cache</param>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        public LocalChunkCache(IGlobalChunkCache globalCache, Index2 dimensions, int range)
        {
            this.globalCache = globalCache;
            this.range = range;

            limitX = dimensions.X;
            limitY = dimensions.Y;
            maskX = (1 << limitX) - 1;
            maskY = (1 << limitY) - 1;
            chunks = new IChunk[(maskX + 1) * (maskY + 1)][];
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

                        int localX = local.X & maskX;
                        int localY = local.Y & maskY;

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
            Flush();

            int height = planet.Size.Z;

            for (int i = 0; i < chunks.Length; i++)
                chunks[i] = new IChunk[height];
        }

        public IChunk GetChunk(PlanetIndex3 index)
        {
            throw new NotImplementedException();
        }

        public ushort GetBlock(Index3 index)
        {
            throw new NotImplementedException();
        }

        public ushort GetBlock(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        public void SetBlock(Index3 index, ushort block)
        {
            throw new NotImplementedException();
        }

        public void SetBlock(int x, int y, int z, ushort block)
        {
            throw new NotImplementedException();
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            for (int i = 0; i < chunks.Length; i++)
            {
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
            return (((y & (maskY)) << limitX) | ((x & (maskX))));
        }
    }
}
