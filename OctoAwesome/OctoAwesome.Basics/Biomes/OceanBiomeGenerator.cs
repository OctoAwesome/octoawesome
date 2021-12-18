namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Biome class or generating the oceanic floor.
    /// </summary>
    public class OceanBiomeGenerator : LargeBiomeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OceanBiomeGenerator"/> class.
        /// </summary>
        /// <param name="planet">The planet the biome should be generated on.</param>
        /// <param name="minValue">The minimum mapping value where the biome is generated.</param>
        /// <param name="maxValue">The maximum mapping value where the biome is generated.</param>
        /// <param name="valueRangeOffset">The value offset the biome height starts at.</param>
        /// <param name="valueRange">The value range the biome height has.</param>
        public OceanBiomeGenerator(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange)
            : base(planet, minValue, maxValue, valueRangeOffset, valueRange, null!) // TODO: currently no noise value is used for the biome floor
        {
        }

        /// <inheritdoc />
        public override void FillHeightmap(Index2 chunkIndex, float[] heightmap)
        {

            chunkIndex = new Index2(chunkIndex.X * Chunk.CHUNKSIZE_X, chunkIndex.Y * Chunk.CHUNKSIZE_Y);

            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    heightmap[(y * Chunk.CHUNKSIZE_X) + x] = 0f;
                }
            }
        }
    }
}
