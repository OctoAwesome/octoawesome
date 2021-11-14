﻿using OctoAwesome.Noise;

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    public class LandBiomeGenerator : LargeBiomeBase
    {
        public LandBiomeGenerator(IPlanet planet, float minVal, float maxVal, float valueRangeOffset, float valueRange)
            : base(planet, valueRangeOffset, valueRange)
        {
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed + 1) { FrequencyX = 1f / 1000, FrequencyY = 1f / 1000, Persistance = 0.25f, Octaves = 5, Factor = 1f };

            MinValue = minVal;
            MaxValue = maxVal;

            SubBiomes.Add(new FlatlandBiome(planet, 0f, 0.2f, 0f, 0.1f));
            SubBiomes.Add(new HillsBiome(planet, 0.3f, 0.5f, 0.1f, 0.4f));
            SubBiomes.Add(new HighMountainBiome(planet, 0.8f, 1f, 0.2f, 0.8f));

            SortSubBiomes();
        }

        public override float[] GetHeightmap(Index2 chunkIndex, float[] heightmap)
        {
            Index2 blockIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            var regions = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (int i = 0; i < regions.Length; i++)
                regions[i] = 0;
            BiomeNoiseGenerator.GetTileableNoiseMap2D(blockIndex.X, blockIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, regions);

            float[] biomeValues = ArrayPool<float>.Shared.Rent(SubBiomes.Count * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);

            var tempArray = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (int i = 0; i < SubBiomes.Count; i++)
            {
                SubBiomes[i].GetHeightmap(chunkIndex, tempArray);
                Array.Copy(tempArray, 0, biomeValues, i * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            }
            ArrayPool<float>.Shared.Return(tempArray);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    float region = (regions[(y * Chunk.CHUNKSIZE_X) + x] / 2) + 0.5f;

                    int biome2;
                    int biome1 = ChooseBiome(region, out biome2);

                    float interpolationValue = 0f;
                    if (biome2 != -1)
                    {
                        interpolationValue = CalculateInterpolationValue(region, SubBiomes[biome1], SubBiomes[biome2]);
                        heightmap[(y * Chunk.CHUNKSIZE_X) + x] = (biomeValues[(biome2 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y) + (y * Chunk.CHUNKSIZE_X) + x] * interpolationValue) + (biomeValues[(biome1 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y) + (y * Chunk.CHUNKSIZE_X) + x] * (1 - interpolationValue));
                    }
                    else
                        heightmap[(y * Chunk.CHUNKSIZE_X) + x] = biomeValues[(biome1 * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y) + (y * Chunk.CHUNKSIZE_X) + x];
                }
            }
            ArrayPool<float>.Shared.Return(regions);
            ArrayPool<float>.Shared.Return(biomeValues);
            return heightmap;
        }
    }
}

