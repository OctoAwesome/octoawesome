using OctoAwesome.Basics.Noise;
using OctoAwesome.Location;

namespace OctoAwesome.Basics.Biomes
{
    public class HillsBiome : BiomeBase
    {
        public HillsBiome(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
            : base(planet, minValue, maxValue, valueRangeOffset, valueRange)
        {
            BiomeNoiseGenerator = new SimplexNoiseGenerator(planet.Seed + 2) { FrequencyX = 1f / 256, FrequencyY = 1f / 256, FrequencyZ = 1f / 256, Persistance = 0.4f, Octaves = 4, Factor = 1f };
        }
    }
}
