using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    public class SurfaceBiomeGenerator : SuperBiomeBase
    {
        public int SeaLevel { get; private set; }

        public SurfaceBiomeGenerator(IPlanet planet, int seaLevel)
            : base(planet, 0f, 1f)
        {
            this.SeaLevel = seaLevel;
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed)
            {
                FrequencyX = 1f / 10000,
                FrequencyY = 1f / 10000,
                Factor = 1f
            };

            float offset = (float)seaLevel / (Planet.Size.Z * Chunk.CHUNKSIZE_Z);

            SubBiomes.Add(new OceanBiomeGenerator(planet, 0f, 0.3f, 0f, offset));
            SubBiomes.Add(new LandBiomeGenerator(planet, 0.5f, 1f, offset, 1 - offset));

            SortSubBiomes();
        }

        protected override float CurveFunction(float inputValue)
        {
            return CurveFunction(inputValue, -0.08f, 200);
        }

        private float CurveFunction(float inputValue, float brightness, int contrast)
        {
            inputValue += brightness;
            float factor = 259f / 255f * (contrast + 255) / (259 - contrast);
            inputValue = factor * (inputValue - 0.5f) + 0.5f;
            return Math.Min(Math.Max(inputValue, 0f), 1f);
        }

        public override float[,] GetHeightmap(Index2 chunkIndex)
        {
            float[,] values = new float[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];

            Index2 blockIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            float[,] regions = BiomeNoiseGenerator.GetTileableNoiseMap2D(blockIndex.X, blockIndex.Y, Chunk.CHUNKSIZE_X,
                Chunk.CHUNKSIZE_Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            float[][,] biomeValues = new float[SubBiomes.Count][,];

            for (int i = 0; i < SubBiomes.Count; i++)
                biomeValues[i] = SubBiomes[i].GetHeightmap(chunkIndex);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    float region = regions[x, y] / 2 + 0.5f;

                    int biome2;
                    int biome1 = ChooseBiome(region, out biome2);

                    float interpolationValue = 0f;
                    if (biome2 != -1)
                    {
                        interpolationValue = CalculateInterpolationValue(region, SubBiomes[biome1], SubBiomes[biome2]);
                        values[x, y] = (biomeValues[biome2][x, y] * interpolationValue) +
                                       (biomeValues[biome1][x, y] * (1 - interpolationValue));
                    }
                    else
                        values[x, y] = biomeValues[biome1][x, y];
                }
            }
            return values;
        }
    }
}