using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using OctoAwesome.Pooling;

using System;
using System.Buffers;
using System.IO;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Map generator used for generating a <see cref="ComplexPlanet"/>.
    /// </summary>
    public class ComplexPlanetGenerator : IMapGenerator
    {
        private readonly ChunkPool chunkPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPlanetGenerator"/> class.
        /// </summary>
        public ComplexPlanetGenerator()
        {
            chunkPool = TypeContainer.Get<ChunkPool>();
        }

        /// <inheritdoc />
        public IPlanet GeneratePlanet(Guid universe, int id, int seed)
            => new ComplexPlanet(id, universe, new Index3(13, 13, 4), this, seed);

        /// <inheritdoc />
        public IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index)
        {
            IDefinition[] definitions = definitionManager.Definitions;
            //TODO More Generic, reconsider complete planet generation (Heatmap + Heightmap + Biome + Modding)

            ushort GetBlockDefinitionInfo<T>()
            {
                var blockIndex = Array.FindIndex(definitions, (x) => x.GetType() == typeof(T));
                return (ushort)(blockIndex + 1);
            }

            ushort sandIndex = GetBlockDefinitionInfo<SandBlockDefinition>();
            ushort snowIndex = GetBlockDefinitionInfo<SnowBlockDefinition>();
            ushort dirtIndex = GetBlockDefinitionInfo<DirtBlockDefinition>();
            ushort stoneIndex = GetBlockDefinitionInfo<StoneBlockDefinition>();
            ushort waterIndex = GetBlockDefinitionInfo<WaterBlockDefinition>();
            ushort grassIndex = GetBlockDefinitionInfo<GrassBlockDefinition>();

            if (planet is not ComplexPlanet localPlanet)
                throw new ArgumentException("planet is not a Type of ComplexPlanet");

            var localHeightmap = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);

            localPlanet.BiomeGenerator.FillHeightmap(index, localHeightmap);

            IChunk[] chunks = new IChunk[planet.Size.Z];
            for (int i = 0; i < planet.Size.Z; i++)
                chunks[i] = chunkPool.Rent(new Index3(index, i), localPlanet.Id);

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
                            int flatIndex = Chunk.GetFlatIndex(x, y, z);
                            int absoluteZ = z + (i * Chunk.CHUNKSIZE_Z);
                            if (absoluteZ <= localHeightmap[(y * Chunk.CHUNKSIZE_X) + x] * localPlanet.Size.Z * Chunk.CHUNKSIZE_Z)
                            {
                                if (obersteSchicht > 0)
                                {
                                    float temp = localPlanet.ClimateMap.GetTemperature(new Index3((index.Y * Chunk.CHUNKSIZE_X) + x, (index.Y * Chunk.CHUNKSIZE_X) + x, (i * Chunk.CHUNKSIZE_Z) + z));

                                    if ((ozeanSurface || surfaceBlock) && (absoluteZ <= (localPlanet.BiomeGenerator.SeaLevel + 2)) && (absoluteZ >= (localPlanet.BiomeGenerator.SeaLevel - 2)))
                                    {

                                        chunks[i].Blocks[flatIndex] = sandIndex;
                                    }
                                    else if (temp >= 35)
                                    {
                                        chunks[i].Blocks[flatIndex] = sandIndex;
                                    }
                                    else if (absoluteZ >= localPlanet.Size.Z * Chunk.CHUNKSIZE_Z * 0.6f)
                                    {
                                        if (temp > 12)
                                            chunks[i].Blocks[flatIndex] = dirtIndex;
                                        else
                                            chunks[i].Blocks[flatIndex] = stoneIndex;
                                    }
                                    else if (temp >= 8)
                                    {
                                        if (surfaceBlock && !ozeanSurface)
                                        {
                                            chunks[i].Blocks[flatIndex] = grassIndex;
                                            surfaceBlock = false;
                                        }
                                        else
                                        {
                                            chunks[i].Blocks[flatIndex] = dirtIndex;
                                        }
                                    }
                                    else if (temp <= 0)
                                    {
                                        if (surfaceBlock && !ozeanSurface)
                                        {
                                            chunks[i].Blocks[flatIndex] = snowIndex;
                                            surfaceBlock = false;
                                        }
                                        else
                                        {
                                            chunks[i].Blocks[flatIndex] = dirtIndex;
                                        }
                                    }
                                    else
                                    {
                                        chunks[i].Blocks[flatIndex] = dirtIndex;
                                    }
                                    obersteSchicht--;
                                }
                                else
                                {
                                    chunks[i].Blocks[flatIndex] = stoneIndex;
                                }
                            }
                            else if ((z + (i * Chunk.CHUNKSIZE_Z)) <= localPlanet.BiomeGenerator.SeaLevel)
                            {

                                chunks[i].Blocks[flatIndex] = waterIndex;
                                ozeanSurface = true;
                            }
                            else
                                chunks[i].Blocks[flatIndex] = 0;
                        }
                    }
                }
            }
            ArrayPool<float>.Shared.Return(localHeightmap);
            ChunkColumn column = new ChunkColumn(chunks, localPlanet.Id, index);
            column.CalculateHeights();
            return column;
        }

        /// <inheritdoc />
        public IPlanet GeneratePlanet(Stream stream)
        {
            IPlanet planet = new ComplexPlanet(this);
            using (var reader = new BinaryReader(stream))
                planet.Deserialize(reader);
            return planet;
        }

        /// <inheritdoc />
        public IChunkColumn GenerateColumn(Stream stream, IPlanet planet, Index2 index)
        {
            IChunkColumn column = new ChunkColumn(planet.Id);
            using (var reader = new BinaryReader(stream))
                column.Deserialize(reader);
            return column;
        }
    }
}
