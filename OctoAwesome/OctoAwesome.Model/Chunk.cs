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

        public TimeSpan LastChange { get; private set; }

        public Chunk(Index3 pos)
        {
            blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];
            Index = pos;
        }

        public IBlock GetBlock(Index3 index)
        {
            if (index.X < 0 || index.X >= Chunk.CHUNKSIZE_X ||
                index.Y < 0 || index.Y >= Chunk.CHUNKSIZE_Y ||
                index.Z < 0 || index.Z >= Chunk.CHUNKSIZE_Z)
                return null;

            return blocks[index.X, index.Y, index.Z];
        }

        public void SetBlock(Index3 index, IBlock block, TimeSpan time)
        {
            if (index.X < 0 || index.X >= Chunk.CHUNKSIZE_X ||
                index.Y < 0 || index.Y >= Chunk.CHUNKSIZE_Y ||
                index.Z < 0 || index.Z >= Chunk.CHUNKSIZE_Z)
                return;

            blocks[index.X, index.Y, index.Z] = block;
            LastChange = time;
        }
    }
}
