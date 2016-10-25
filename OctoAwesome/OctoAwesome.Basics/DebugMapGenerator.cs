﻿using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OctoAwesome.Basics.Definitions.Blocks;

namespace OctoAwesome.Basics
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(Guid universe, int id, int seed)
        {
            Planet planet = new Planet(id, universe, new Index3(4, 4, 3), seed);
            planet.Generator = this;
            return planet;
        }

        public IChunkColumn GenerateColumn(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, Index2 index)
        {
            IBlockDefinition sandDefinition = blockDefinitions.FirstOrDefault(d => typeof(SandBlockDefinition) == d.GetType());
            ushort sandIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), sandDefinition) + 1);

            IChunk[] result = new IChunk[planet.Size.Z];

            ChunkColumn column = new ChunkColumn(result, planet.Id, index);


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
                            result[layer].SetBlock(x, y, block, sandIndex);
                        }
                    }
                }
            }

            column.CalculateHeights();
            return column;
        }

        public IPlanet GeneratePlanet(Stream stream)
        {
            IPlanet planet = new Planet();
            planet.Deserialize(stream);
            return planet;
        }

        

        public IChunkColumn GenerateColumn(Stream stream, IDefinitionManager definitionManager, int planetId, Index2 index)
        {
            IChunkColumn column = new ChunkColumn();
            column.Deserialize(stream, definitionManager, planetId, index);
            return column;
        }
    }
}
