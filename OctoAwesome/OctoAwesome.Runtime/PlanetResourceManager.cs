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
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public ushort GetBlock(int x, int y, int z)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            //TODO: Prüfen, ob dieser Fall eintreten darf. 
            if (chunk != null) 
                return chunk.GetBlock(x, y, z);
            
            return 0;
        }

        [Obsolete]
        public void SetBlock(Index3 index, ushort block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, ushort block)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            chunk.SetBlock(x, y, z, block);
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            return chunk.GetBlockMeta(x, y, z);
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            var chunk = _chunkCache.Get(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);
            chunk.SetBlockMeta(x, y, z, meta);
        }
    }
}
