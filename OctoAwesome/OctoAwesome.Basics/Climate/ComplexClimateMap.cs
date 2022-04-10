using System;
using OctoAwesome.Basics.Noise;

namespace OctoAwesome.Basics.Climate
{
    /// <summary>
    /// Climate map for <see cref="ComplexPlanet"/> implementation.
    /// </summary>
    public class ComplexClimateMap : IClimateMap
    {
        /// <inheritdoc />
        public IPlanet Planet => planet;

        private readonly ComplexPlanet planet;
        private readonly INoise tempFluctuationGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexClimateMap"/> class.
        /// </summary>
        /// <param name="planet">The planet the complex climate map is for.</param>
        public ComplexClimateMap(ComplexPlanet planet)
        {
            this.planet = planet;
            tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) { Octaves = 3 };
        }

        /// <inheritdoc />
        public float GetTemperature(Index3 blockIndex)
        {
            int equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            float equatorTemperature = 40f;
            float poleTemperature = -20f;
            float tempFluctuation = tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            float temperatureDifference = poleTemperature - equatorTemperature;
            float temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            float temperature = tempFluctuation + equatorTemperature + temperatureDifference * (float)Math.Sin((Math.PI / 2) * distance / equator);  //equatorTemperature + distance * temperatureDifference / equator;
            float height = (float)(blockIndex.Z - planet.BiomeGenerator.SeaLevel) / (Planet.Size.Z * Chunk.CHUNKSIZE_Z - planet.BiomeGenerator.SeaLevel);
            height = Math.Max(height, 0);
            height = height * height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        /// <inheritdoc />
        public int GetPrecipitation(Index3 blockIndex)
        {
            int maxPrecipitation = 100;

            float rawValue = planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y, Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            int height = blockIndex.Z - planet.BiomeGenerator.SeaLevel;
            float precipitationDecreasePerBlock = 1;

            return (int)(((1 - rawValue) * maxPrecipitation) - (Math.Max(height, 0) * precipitationDecreasePerBlock));
        }
    }
}
