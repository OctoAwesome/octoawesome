using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    class FlatlandBiome : BiomeBase
    {
        public FlatlandBiome(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
            : base(planet, minValue, maxValue, valueRangeOffset, valueRange)
        {
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed + 2) { FrequencyX = 1f / 256, FrequencyY = 1f / 256, Persistance = 0.25f, Octaves = 3, Factor = 1f };

        }
    }
}
