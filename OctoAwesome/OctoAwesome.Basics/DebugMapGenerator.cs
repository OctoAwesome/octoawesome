using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IUniverse GenerateUniverse(int id)
        {
            return new Universe(id, "Milchstrasse");
        }

        public IPlanet GeneratePlanet(int universe, int seed)
        {
            return new Planet(0, universe, new Index3(1000, 1000, 3), seed);
        }

        public IChunk[] GenerateChunk(IPlanet planet, Index2 index)
        {
            IChunk[] result = new IChunk[planet.Size.Z];

            for (int layer = 0; layer < planet.Size.Z; layer++)
                result[layer] = new Chunk(new Index3(index.X, index.Y, layer), planet.Id);

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
                            int layer = (int)(z / Chunk.CHUNKSIZE_Z);
                            result[layer].SetBlock(x, y, block, new SandBlock(new Index3(
                                (index.X * Chunk.CHUNKSIZE_X) + x, 
                                (index.Y * Chunk.CHUNKSIZE_Y) + y, 
                                (layer * Chunk.CHUNKSIZE_Z) + block)));
                        }
                    }
                }
            }

            return result;
        }
    }
}
