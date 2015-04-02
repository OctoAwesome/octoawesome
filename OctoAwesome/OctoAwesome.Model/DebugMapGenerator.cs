using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(int seed)
        {
            return new Planet(new Index3(1000, 1000, 3), this, seed);
        }

        public IChunk[] GenerateChunk(IPlanet planet, Index2 index)
        {
            IChunk[] result = new IChunk[planet.Size.Z];

            for (int layer = 0; layer < planet.Size.Z; layer++)
                result[layer] = new Chunk(new Index3(index.X, index.Y, layer), planet);

            int part = (planet.Size.Z * Chunk.CHUNKSIZE_Z) / 4;

            for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
            {
                float heightY = (float)Math.Sin((float)(y * Math.PI) / 15f);
                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                {
                    float heightX = (float)Math.Sin((float)(x * Math.PI) / 18f);

                    float height = ((heightX + heightY + 2) / 4) * (2 * part);
                    for (int z = 0; z < planet.Size.Z * Chunk.CHUNKSIZE_Z; z++)
                    {
                        if (z < (int)(height + part))
                        {
                            int block = z % (Chunk.CHUNKSIZE_Z);
                            int layer = (int)((z - block) / Chunk.CHUNKSIZE_Z);
                            result[layer].SetBlock(x, y, block, new SandBlock());
                        }
                    }
                }
            }

            return result;
        }
    }
}
