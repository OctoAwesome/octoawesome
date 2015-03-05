using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Chunk
    {
        public const int CHUNKSIZE_X = 32;
        public const int CHUNKSIZE_Y = 32;
        public const int CHUNKSIZE_Z = 32;

        private IBlock[, ,] blocks;

        public Index3 ChunkPosition { get; private set; }

        public TimeSpan LastChange { get; private set; }

        public Chunk(Index3 pos)
        {
            blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];
            ChunkPosition = pos;

            for (int z = 0; z < CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < CHUNKSIZE_Y; y++)
                {
                    float heightY = (float)Math.Sin((float)(y * Math.PI) / 16f);
                    for (int x = 0; x < CHUNKSIZE_X; x++)
                    {
                        float heightX = (float)Math.Sin((float)(x * Math.PI) / 16f);

                        float height = (heightX + heightY) * 2;


                        if (z < (int)(16 + height))
                        {
                            //if (x % 2 == 0 || y % 2 == 0)
                            blocks[x, y, z] = new GrassBlock();
                            //else
                            //    Blocks[x, y, z] = new SandBlock();
                        }
                    }
                }
            }
        }

        public IBlock GetBlock(Index3 pos)
        {
            return GetBlock(pos.X, pos.Y, pos.Z);
        }

        public IBlock GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= Chunk.CHUNKSIZE_X || 
                y < 0 || y >= Chunk.CHUNKSIZE_Y || 
                z < 0 || z >= Chunk.CHUNKSIZE_Z)
                throw new IndexOutOfRangeException();

            return blocks[x, y, z];
        }

        public void SetBlock(Index3 pos, IBlock block, TimeSpan time)
        {
            SetBlock(pos.X, pos.Y, pos.Z, block, time);
        }

        public void SetBlock(int x, int y, int z, IBlock block, TimeSpan time)
        {
            blocks[x, y, z] = block;
            LastChange = time;
        }
    }
}
