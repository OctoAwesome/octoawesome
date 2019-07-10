using OctoAwesome.Basics.Definitions.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Basics
{
    public class ComplexPlanetGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(Guid universe, int id, int seed) 
            => new ComplexPlanet(id, universe, new Index3(12, 12, 3), this, seed);

        public IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index)
        {
            IDefinition[] definitions = definitionManager.GetDefinitions().ToArray();
            //TODO More Generic, überdenken der Planetgeneration im allgemeinen (Heapmap + Highmap + Biome + Modding)
            IBlockDefinition sandDefinition = definitions.OfType<SandBlockDefinition>().FirstOrDefault();
            ushort sandIndex = (ushort)(Array.IndexOf(definitions.ToArray(), sandDefinition) + 1);

            IBlockDefinition snowDefinition = definitions.OfType<SnowBlockDefinition>().FirstOrDefault();
            ushort snowIndex = (ushort)(Array.IndexOf(definitions.ToArray(), snowDefinition) + 1);

            IBlockDefinition groundDefinition = definitions.OfType<GroundBlockDefinition>().FirstOrDefault();
            ushort groundIndex = (ushort)(Array.IndexOf(definitions.ToArray(), groundDefinition) + 1);

            IBlockDefinition stoneDefinition = definitions.OfType<StoneBlockDefinition>().FirstOrDefault();
            ushort stoneIndex = (ushort)(Array.IndexOf(definitions.ToArray(), stoneDefinition) + 1);

            IBlockDefinition waterDefinition = definitions.OfType<WaterBlockDefinition>().FirstOrDefault();
            ushort waterIndex = (ushort)(Array.IndexOf(definitions.ToArray(), waterDefinition) + 1);

            IBlockDefinition grassDefinition = definitions.OfType<GrassBlockDefinition>().FirstOrDefault();
            ushort grassIndex = (ushort)(Array.IndexOf(definitions.ToArray(), grassDefinition) + 1);

            if (!(planet is ComplexPlanet))
                throw new ArgumentException("planet is not a Type of ComplexPlanet");

            ComplexPlanet localPlanet = (ComplexPlanet)planet;

            float[,] localHeightmap = localPlanet.BiomeGenerator.GetHeightmap(index);

            IChunk[] chunks = new IChunk[planet.Size.Z];
            for (int i = 0; i < planet.Size.Z; i++)
                chunks[i] = new Chunk(new Index3(index, i), planet.Id);

            int obersteSchicht;
            bool surfaceBlock;
            bool ozeanSurface;

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    obersteSchicht = 5;
                    surfaceBlock = true;
                    ozeanSurface = false;

                    for (int i = chunks.Length - 1; i >= 0; i--)
                    {
                        for (int z = Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
                        {
                            int absoluteZ = (z + (i * Chunk.CHUNKSIZE_Z));

                            if (absoluteZ <= localHeightmap[x, y] * localPlanet.Size.Z * Chunk.CHUNKSIZE_Z)
                            {
                                if (obersteSchicht > 0)
                                {
                                    float temp = localPlanet.ClimateMap.GetTemperature(new Index3(index.X * Chunk.CHUNKSIZE_X + x, index.Y * Chunk.CHUNKSIZE_Y + y, i * Chunk.CHUNKSIZE_Z + z));

                                    if ((ozeanSurface || surfaceBlock) && (absoluteZ <= (localPlanet.BiomeGenerator.SeaLevel + 2)) && (absoluteZ >= (localPlanet.BiomeGenerator.SeaLevel - 2)))
                                    {
                                        chunks[i].SetBlock(x, y, z, sandIndex);
                                    }
                                    else if (temp >= 35)
                                    {
                                        chunks[i].SetBlock(x, y, z, sandIndex);
                                    }
                                    else if (absoluteZ >= localPlanet.Size.Z * Chunk.CHUNKSIZE_Z * 0.6f)
                                    {
                                        if (temp > 12)
                                            chunks[i].SetBlock(x, y, z, groundIndex);
                                        else
                                            chunks[i].SetBlock(x, y, z, stoneIndex);
                                    }
                                    else if (temp >= 8)
                                    {
                                        if (surfaceBlock && !ozeanSurface)
                                        {
                                            chunks[i].SetBlock(x, y, z, grassIndex);
                                            surfaceBlock = false;
                                        }
                                        else
                                        {
                                            chunks[i].SetBlock(x, y, z, groundIndex);
                                        }
                                    }
                                    else if (temp <= 0)
                                    {
                                        if (surfaceBlock && !ozeanSurface)
                                        {
                                            chunks[i].SetBlock(x, y, z, snowIndex);
                                            surfaceBlock = false;
                                        }
                                        else
                                        {
                                            chunks[i].SetBlock(x, y, z, groundIndex);
                                        }
                                    }
                                    else
                                    {
                                        chunks[i].SetBlock(x, y, z, groundIndex);
                                    }
                                    obersteSchicht--;
                                }
                                else
                                {
                                    chunks[i].SetBlock(x, y, z, stoneIndex);
                                }
                            }
                            else if ((z + (i * Chunk.CHUNKSIZE_Z)) <= localPlanet.BiomeGenerator.SeaLevel)
                            {

                                chunks[i].SetBlock(x, y, z, waterIndex);
                                ozeanSurface = true;
                            }

                        }
                    }
                }
            }

            ChunkColumn column = new ChunkColumn(chunks, planet.Id, index);
            column.CalculateHeights();
            return column;
        }

        public IPlanet GeneratePlanet(Stream stream)
        {
            IPlanet planet = new ComplexPlanet();
            using(var reader = new BinaryReader(stream))
                planet.Deserialize(reader);
            planet.Generator = this;
            return planet;
        }

        public IChunkColumn GenerateColumn(Stream stream, int planetId, Index2 index)
        {
            IChunkColumn column = new ChunkColumn();
            using(var reader = new BinaryReader(stream))
                column.Deserialize(reader);
            return column;
        }
    }
}
