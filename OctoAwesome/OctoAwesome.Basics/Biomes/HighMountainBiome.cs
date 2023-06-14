using OctoAwesome.Basics.Noise;
using OctoAwesome.Location;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Biome that generates high mountains.
    /// </summary>
    public class HighMountainBiome : BiomeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BiomeBase"/> class.
        /// </summary>
        /// <param name="planet">The planet the biome should be generated on.</param>
        /// <param name="minValue">The minimum mapping value where the biome is generated.</param>
        /// <param name="maxValue">The maximum mapping value where the biome is generated.</param>
        /// <param name="valueRangeOffset">The value offset the biome height starts at.</param>
        /// <param name="valueRange">The value range the biome height has.</param>
        public HighMountainBiome(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
            : base(planet, minValue, maxValue, valueRangeOffset, valueRange,
                new SimplexNoiseGenerator(planet.Seed + 2) { FrequencyX = 1f / 256, FrequencyY = 1f / 256, FrequencyZ = 1f / 256, Persistence = 0.5f, Octaves = 6, Factor = 1f })
        {
        }
    }
}
