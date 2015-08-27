using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class PlanetResourceManager : IPlanetResourceManager
    {
        private readonly IChunkCache _chunkCache;

        public PlanetResourceManager(IChunkCache chunkCache)
        {
            _chunkCache = chunkCache;
        }

        internal IChunkCache ChunkCache { get{ return _chunkCache;} }

        public IChunk GetChunk(Index3 index)
        {
            return _chunkCache.Get(index);
        }

        [Obsolete]
        public BlockDefinition GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public BlockDefinition GetBlock(int x, int y, int z)
        {
            var chunk = _chunkCache.Get(x, y, z);
            return chunk.GetBlock(x, y, z);
        }

        [Obsolete]
        public void SetBlock(Index3 index, BlockDefinition block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, BlockDefinition block)
        {
            var chunk = _chunkCache.Get(x, y, z);
            chunk.SetBlock(x, y, z, block);
        }
    }
}
