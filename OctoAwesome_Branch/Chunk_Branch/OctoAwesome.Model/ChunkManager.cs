using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noises;
using OctoAwesome.Model.Blocks;

namespace OctoAwesome.Model
{
    public sealed class ChunkManager
    {

        public List<Chunk> Chunks { get; private set; }
        public INoise NoiseGenerator { get; private set; }

        public ChunkManager(INoise noiseGenerator)
        {
            this.NoiseGenerator = noiseGenerator;     

            Chunks = new List<Chunk>();

            for (int x = -5; x < 5; x++)
            {
                for (int z = -5; z < 5; z++)
                {
                    Chunks.Add(new Chunk(x, 0, z) { Blocks = CreateChunk(x, 0, z) });
                }
            }

        }

        public IBlock[, ,] CreateChunk(int chunkX, int chunkY, int chunkZ)
        {

            IBlock[, ,] blocks = new IBlock[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_Z];

            float[, ,] noise = NoiseGenerator.GetNoise3(chunkX, chunkY, chunkZ, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_Z);

            for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            {
                float min_noise = (float)Math.Cos((((double)y * Math.PI) / 50d) + Math.PI);
               // min_noise = 0f;

                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                {
                    for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                    {
                    
                             if ((noise[x, y, z]) > min_noise)
                            blocks[x, y, z] = new GrassBlock();
                        //else if ((noise[x, y, z] / 2) < 0.2f)
                        //    blocks[x, y, z] = new GrassBlock();
                    }
                }
            }


            return blocks;
        }

        public Blocks.IBlock GetBlock(int x, int y, int z)
        {

            int chunkX = (int)(x / Chunk.CHUNKSIZE_X);
            int chunkY = (int)(y / Chunk.CHUNKSIZE_Y);
            int chunkZ = (int)(z / Chunk.CHUNKSIZE_Z);

            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            z %= Chunk.CHUNKSIZE_Z;

            if (x < 0)
            {
                chunkX--;
                x += 50;
            }
            if (y < 0)
            {
                chunkY--;
                y += 50;
            }
            if (z < 0)
            {
                chunkZ--;
                z += 50;
            }



            foreach (Chunk chunk in Chunks)
            {

                if (chunk.Chunk_X == chunkX && chunk.Chunk_Y == chunkY && chunk.Chunk_Z == chunkZ)
                {

                    return chunk.Blocks[x, y, z];
                }
            }

            return null;
        }

        public Chunk GetChunk(int chunkX, int chunkY, int chunkZ)
        {

            foreach (Chunk chunk in Chunks)
            {
                if (chunk.Chunk_X == chunkX && chunk.Chunk_Y == chunkY && chunk.Chunk_Z == chunkZ)
                {
                    return chunk;
                }
            }

            return null;
        }

    }
}
