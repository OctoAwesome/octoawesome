using OctoAwesome.Basics.Noise;
using OctoAwesome.Location;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Interface for biomes used for biome generation.
    /// </summary>
    public interface IBiome
    {
        /// <summary>
        /// Gets the planet the biome is generated for.
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Gets the minimum mapping value the biome starts at [0..1].
        /// </summary>
        float MinValue { get; }

        /// <summary>
        /// Gets the maximum mapping value the biome ends at [0..1].
        /// </summary>
        float MaxValue { get; }

        /// <summary>
        /// Gets the height value range offset the biome starts at.
        /// </summary>
        float ValueRangeOffset { get; }

        /// <summary>
        /// Gets the height value range the biome has.
        /// </summary>
        float ValueRange { get; }

        /// <summary>
        /// Gets the noise generator used for generating the biome features.
        /// </summary>
        INoise BiomeNoiseGenerator { get; }

        /// <summary>
        /// Read the heightmap values for this biome into a flat heightmap array.
        /// </summary>
        /// <param name="chunkIndex">The chunk position to read the heightmap at.</param>
        /// <param name="heightmap">
        /// The heightmap array to read into.
        /// Should be size <see cref="Chunk.CHUNKSIZE_X"/> * <see cref="Chunk.CHUNKSIZE_Y"/>.
        /// </param>
        void FillHeightmap(Index2 chunkIndex, float[] heightmap);
    }
}
