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
            return new Planet(new Index3(10, 10, 1), this, seed);
        }

        public IChunk[] GenerateChunk(IPlanet planet, Index2 index)
        {
            IChunk[] result = new IChunk[planet.Size.Z];

            for (int layer = 0; layer < planet.Size.Z; layer++)
            {
                result[layer] = new Chunk(new Index3(index.X, index.Y, layer));

                for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                {
                    for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                    {
                        float heightY = (float)Math.Sin((float)(y * Math.PI) / 32f);
                        for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        {
                            float heightX = (float)Math.Sin((float)(x * Math.PI) / 32f);

                            float height = (heightX + heightY) * 2;


                            if (z < (int)(16 + height))
                            {
                                result[layer].SetBlock(x, y, z, new WaterBlock());
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
