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
        private IResourceManager resourceManager;

        private readonly IChunk[][] chunks;

        private int? planet;
        private int limitX;
        private int limitY;
        private int maskX;
        private int maskY;

        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="globalCache">Referenz auf global Chunk Cache</param>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        public LocalChunkCache(IResourceManager resourceManager, IGlobalChunkCache globalCache, Index2 dimensions)
        {
            this.resourceManager = resourceManager;

            limitX = dimensions.X;
            limitY = dimensions.Y;
            maskX = (1 << limitX) - 1;
            maskY = (1 << limitY) - 1;
            chunks = new IChunk[(maskX + 1) * (maskY + 1)][];
        }

        public void SetCenter(PlanetIndex3 index)
        {
            // Planet resetten falls notwendig
            if (index.Planet != planet)
                InitializePlanet(index.Planet);

        }

        private void InitializePlanet(int planet)
        {
            Flush();

            IPlanet p = resourceManager.GetPlanet(planet);
            int height = p.Size.Z;

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
            throw new NotImplementedException();
        }

        private int FlatIndex(int x, int y)
        {
            return (((y & (maskY)) << limitX) | ((x & (maskX))));
        }
    }
}
