using System.Buffers;
using System.Collections.Generic;
using OctoAwesome.Basics.Noise;

namespace OctoAwesome.Basics.Biomes
{

    public abstract class BiomeBase : IBiome
    {
        public IPlanet Planet { get; }

        public List<IBiome> SubBiomes { get; }

        public INoise BiomeNoiseGenerator { get; }

        public float MinValue { get; }

        public float MaxValue { get; }

        public float ValueRangeOffset { get; }

        public float ValueRange { get; }

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
