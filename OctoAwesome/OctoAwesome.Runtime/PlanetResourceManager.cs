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
        public IBlock GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            //TODO: Prüfen, ob dieser Fall eintreten darf. 
            if (chunk != null) 
                return chunk.GetBlock(x, y, z);
            
            return null;
        }

        [Obsolete]
        public void SetBlock(Index3 index, IBlock block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            chunk.SetBlock(x, y, z, block);
        }
    }
}
