using System;
using System.Linq;
using System.IO;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using OctoAwesome.Chunking;
using OctoAwesome.Location;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Debug map generator for testing the map generation process.
    /// </summary>
    public class DebugMapGenerator : IMapGenerator
    {
        /// <inheritdoc />
        public IPlanet GeneratePlanet(Guid universe, int id, int seed)
        {
            Planet planet = new Planet(id, universe, new Index3(5, 5, 4), this, seed);
            return planet;
        }

        /// <inheritdoc />
        public IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index)
        {
            IDefinition[] definitions = definitionManager.Definitions.ToArray();

            IBlockDefinition sandDefinition = definitions.OfType<SandBlockDefinition>().First();
            ushort sandIndex = (ushort)(Array.IndexOf(definitions.ToArray(), sandDefinition) + 1);

            IChunk[] result = new IChunk[planet.Size.Z];

            ChunkColumn column = new ChunkColumn(result, planet, index);


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
                            int layer = z / Chunk.CHUNKSIZE_Z;
                            result[layer].SetBlock(x, y, block, sandIndex);
                        }
                    }
                }
            }

            column.CalculateHeights();
            return column;
        }

        /// <inheritdoc />
        public IPlanet GeneratePlanet(Stream stream)
        {
            IPlanet planet = new Planet(this);
            using (var reader = new BinaryReader(stream))
                planet.Deserialize(reader);
            return planet;
        }

        /// <inheritdoc />
        public IChunkColumn GenerateColumn(Stream stream, IPlanet planet, Index2 index)
        {
            IChunkColumn column = new ChunkColumn(planet);
            using (var reader = new BinaryReader(stream))
                column.Deserialize(reader);
            return column;
        }
    }
}
