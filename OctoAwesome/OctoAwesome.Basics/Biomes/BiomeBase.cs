using OctoAwesome.Basics.Noise;

using System;
using System.Buffers;
using System.Collections.Generic;
using OctoAwesome.Basics.Noise;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Base class for biomes.
    /// </summary>
    public abstract class BiomeBase : IBiome
    {
        /// <inheritdoc />
        public IPlanet Planet { get; }

        /// <summary>
        /// Gets a list of sub biomes this biome can consist of.
        /// </summary>
        public List<IBiome> SubBiomes { get; }

        /// <inheritdoc />
        public INoise BiomeNoiseGenerator { get; }

        /// <inheritdoc />
        public float MinValue { get; }

        /// <inheritdoc />
        public float MaxValue { get; }

        /// <inheritdoc />
        public float ValueRangeOffset { get; }

        /// <inheritdoc />
        public float ValueRange { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BiomeBase"/> class.
        /// </summary>
        /// <param name="planet">The planet the biome should be generated on.</param>
        /// <param name="minValue">The minimum mapping value where the biome is generated.</param>
        /// <param name="maxValue">The maximum mapping value where the biome is generated.</param>
        /// <param name="valueRangeOffset">The value offset the biome height starts at.</param>
        /// <param name="valueRange">The value range the biome height has.</param>
        /// <param name="biomeNoiseGenerator">The noise generator used for generating the biome features.</param>
        protected BiomeBase(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange, INoise biomeNoiseGenerator)
        {
            SubBiomes = new List<IBiome>();
            Planet = planet;
            MinValue = minValue;
            MaxValue = maxValue;
            ValueRangeOffset = valueRangeOffset;
            ValueRange = valueRange;
            BiomeNoiseGenerator = biomeNoiseGenerator;
        }

        /// <inheritdoc />
        public virtual void FillHeightmap(Index2 chunkIndex, float[] heightmap)
        {
            chunkIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);
            float[] heights = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (int i = 0; i < heights.Length; i++)
                heights[i] = 0;
            BiomeNoiseGenerator.FillTileableNoiseMap2D(chunkIndex.X, chunkIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, heights);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    heightmap[(y * Chunk.CHUNKSIZE_X) + x] = (((heights[(y * Chunk.CHUNKSIZE_X) + x] / 2) + 0.5f) * ValueRange) + ValueRangeOffset;
                }
            }
            ArrayPool<float>.Shared.Return(heights);
        }
    }
}
