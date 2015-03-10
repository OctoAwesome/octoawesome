using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Chunk : IChunk
    {
        public const int CHUNKSIZE_X = 32;
        public const int CHUNKSIZE_Y = 32;
        public const int CHUNKSIZE_Z = 32;

        protected IBlock[, ,] blocks;

        public Index3 Index { get; private set; }

        public int ChangeCounter { get; private set; }

        public Chunk(Index3 pos)
        {
            blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];
            Index = pos;
            ChangeCounter = 0;
        }

        public IBlock GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNKSIZE_X ||
                y < 0 || y >= Chunk.CHUNKSIZE_Y ||
                z < 0 || z >= Chunk.CHUNKSIZE_Z)
                return null;

            return blocks[x, y, z];
        }

        public void SetBlock(Index3 index, IBlock block)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        public void SetBlock(int x, int y, int z, IBlock block)
        {
            if (x < 0 || x >= Chunk.CHUNKSIZE_X ||
                y < 0 || y >= Chunk.CHUNKSIZE_Y ||
                z < 0 || z >= Chunk.CHUNKSIZE_Z)
                return;

            blocks[x, y, z] = block;
            ChangeCounter++;
        }
    }
}
