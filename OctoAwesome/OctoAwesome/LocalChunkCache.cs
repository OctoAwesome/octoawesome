using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class LocalChunkCache
    {
        public LocalChunkCache(IGlobalChunkCache globalCache, Index3 dimensions)
        {

        }

        public void SetCenter(PlanetIndex3 index)
        {

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
    }
}
