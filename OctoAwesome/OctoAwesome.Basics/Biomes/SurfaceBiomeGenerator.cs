﻿using OctoAwesome.Basics.Noise;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Biome generator root that generates the whole planet.
    /// </summary>
    public class SurfaceBiomeGenerator : LargeBiomeBase
    {
        /// <summary>
        /// Gets the sea level the land mass starts generating at (in blocks).
        /// </summary>
        public int SeaLevel
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceBiomeGenerator"/> class.
        /// </summary>
        /// <param name="planet">The planet the biome should be generated on.</param>
        /// <param name="seaLevel">The height level the landmass should start generating at (in blocks).</param>
        public SurfaceBiomeGenerator(IPlanet planet, int seaLevel)
            : base(planet, 0f, 1f, 0f, 1f,
                new SimplexNoiseGenerator(planet.Seed) { FrequencyX = 1f / 10000, FrequencyY = 1f / 10000, Factor = 1f })
        {

            SeaLevel = seaLevel;

            float offset = (float)seaLevel / (Planet.Size.Z * Chunk.CHUNKSIZE_Z);

            SubBiomes.Add(new OceanBiomeGenerator(planet, 0f, 0.3f, 0f, offset));
            SubBiomes.Add(new LandBiomeGenerator(planet, 0.5f, 1f, offset, 1 - offset));

            SortSubBiomes();
        }

        /// <inheritdoc />
        protected override float CurveFunction(float inputValue)
        {
            return CurveFunction(inputValue, -0.08f, 200);
        }

        private float CurveFunction(float inputValue, float brightness, int contrast)
        {
            inputValue += brightness;
            float factor = 259f / 255f * (contrast + 255) / (259 - contrast);
            inputValue = (factor * (inputValue - 0.5f)) + 0.5f;
            return Math.Min(Math.Max(inputValue, 0f), 1f);
        }

        /// <inheritdoc />
        public override void FillHeightmap(Index2 chunkIndex, float[] heightmap)
        {
            Index2 blockIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            var regions = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (int i = 0; i < regions.Length; i++)
                regions[i] = 0;
            BiomeNoiseGenerator.FillTileableNoiseMap2D(blockIndex.X, blockIndex.Y, Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y, regions);

            float[] biomeValues = ArrayPool<float>.Shared.Rent(SubBiomes.Count * Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y); //float[SubBiomes.Count][,]

            var tempArray = ArrayPool<float>.Shared.Rent(Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_Y);
            for (int i = 0; i < SubBiomes.Count; i++)
            {
                SubBiomes[i].FillHeightmap(chunkIndex, tempArray);
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

                    float interpolationValue;
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
        }
    }
}
