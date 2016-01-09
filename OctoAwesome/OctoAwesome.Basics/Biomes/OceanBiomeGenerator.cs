using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    public class OceanBiomeGenerator : IBiome
    {
        public OceanBiomeGenerator(IPlanet planet, float minVal, float maxVal, float valueRangeOffset, float valueRange)
        {
            this.Planet = planet;
            this.MinValue = minVal;
            this.MaxValue = maxVal;
            this.ValueRangeOffset = valueRangeOffset;
            this.ValueRange = valueRange;
        }

        public IPlanet Planet { get; private set; }

        public List<IBiome> SubBiomes { get; private set; }

        public INoise BiomeNoiseGenerator { get; private set; }

        public float MinValue { get; private set; }

        public float MaxValue { get; private set; }

        public float ValueRangeOffset { get; private set; }

        public float ValueRange { get; private set; }

        public float[,] GetHeightmap(Index2 chunkIndex)
        {
            float[,] values = new float[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];

            chunkIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    values[x, y] = 0f;
                }
            }
            return values;
        }
    }
}