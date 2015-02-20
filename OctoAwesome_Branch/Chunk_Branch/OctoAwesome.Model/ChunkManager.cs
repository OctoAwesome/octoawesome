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

            for (int x = -2; x < 2; x++)
            {
                for (int z = -2; z < 2; z++)
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
                double min_noise = Math.Cos(((y / 50f) * Math.PI) + Math.PI);

                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                {
                    for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                    {

                        if (y < 2)
                        {
                            blocks[x, y, z] = new SandBlock();
                            continue;
                        }
                        if ((noise[x, y, z]) > 0) blocks[x, y, z] = new GrassBlock();
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
