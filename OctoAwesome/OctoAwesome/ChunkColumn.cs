using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class ChunkColumn : IChunkColumn
    {
        public ChunkColumn(IChunk[] chunks,int planet,Index2 columnIndex)
        {
            Planet = planet;
            Chunks = chunks;
            Index = columnIndex;
        }

        public IChunk[] Chunks
        {
            get;
            private set;
        }

        public bool Populated
        {
            get;
            private set;
        }
        public int Planet
        {
            get;
            private set;
        }
        public Index2 Index
        {
            get;
            private set;
        }
        

        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public ushort GetBlock(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlock(x, y, z);
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockMeta(x, y, z);
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockResources(x, y, z);
        }

        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block, meta);
        }

        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlock(x, y, z,block,meta);
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockMeta(x, y, z,  meta);
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockResources(x, y, z, resources);
        }
    }
}
