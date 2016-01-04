using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    class HighMountainBiome : IBiome
    {
        public IPlanet Planet { get; private set; }

        public List<IBiome> SubBiomes { get; private set; }

        public INoise BiomeNoiseGenerator { get; private set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }

        public float ValueRangeOffset { get; private set; }

        public float ValueRange { get; private set; }

        public HighMountainBiome(IPlanet planet, float minValue, float maxValue, float valueRangeOffset,
            float valueRange)
        {
            this.BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed + 2)
            {
                FrequencyX = 1f/256,
                FrequencyY = 1f/256,
                FrequencyZ = 1f/256,
                Persistance = 0.5f,
                Octaves = 6,
                Factor = 1f
            };
            this.Planet = planet;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.ValueRangeOffset = valueRangeOffset;
            this.ValueRange = valueRange;
        }

        public float[,] GetHeightmap(Index2 chunkIndex)
        {
            float[,] values = new float[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];

            chunkIndex = new Index2(chunkIndex.X*Chunk.CHUNKSIZE_X, chunkIndex.Y*Chunk.CHUNKSIZE_Y);

            float[,] heights = BiomeNoiseGenerator.GetTileableNoiseMap2D(chunkIndex.X, chunkIndex.Y, Chunk.CHUNKSIZE_X,
                Chunk.CHUNKSIZE_Y, Planet.Size.X*Chunk.CHUNKSIZE_X, Planet.Size.Y*Chunk.CHUNKSIZE_Y);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    values[x, y] = (heights[x, y]/2 + 0.5f)*ValueRange + ValueRangeOffset;
                }
            }
            return values;
        }
    }
}