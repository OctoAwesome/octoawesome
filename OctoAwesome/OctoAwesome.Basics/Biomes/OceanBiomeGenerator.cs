using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    public class OceanBiomeGenerator : LargeBiomeBase
    {
        public OceanBiomeGenerator(IPlanet planet, float minVal, float maxVal, float valueRangeOffset, float valueRange)
            :base(planet, valueRangeOffset, valueRange)
        {
            MinValue = minVal;
            MaxValue = maxVal;
        }

        public override float[,] GetHeightmap(Index2 chunkIndex)
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
