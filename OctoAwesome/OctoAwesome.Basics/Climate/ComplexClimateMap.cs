using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Climate
{
    public class ComplexClimateMap : IClimateMap
    {
        ComplexPlanet planet;

        public IPlanet Planet
        {
            get { return planet; }
        }

        public ComplexClimateMap(ComplexPlanet planet)
        {
            this.planet = planet;
            tempFluctuationGenerator = new SimplexNoiseGenerator(planet.Seed - 1, 1f / 64, 1f / 64) {Octaves = 3};
        }

        private INoise tempFluctuationGenerator;


        public float GetTemperature(Index3 blockIndex)
        {
            int equator = (Planet.Size.Y * Chunk.CHUNKSIZE_Y) / 2;
            float equatorTemperature = 40f;
            float poleTemperature = -10f;
            float tempFluctuation =
                tempFluctuationGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y,
                    Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y) * 5f;
            float temperatureDifference = poleTemperature - equatorTemperature;
            float temperatureDecreasePerBlock = 0.1f;
            float distance = Math.Abs(blockIndex.Y - equator);
            float temperature = tempFluctuation + equatorTemperature +
                                temperatureDifference * (float)Math.Sin((Math.PI / 2) * distance / equator);
                //equatorTemperature + distance * temperatureDifference / equator;
            float height = (blockIndex.Z - planet.BiomeGenerator.SeaLevel) /
                           (Planet.Size.Z * Chunk.CHUNKSIZE_Z - planet.BiomeGenerator.SeaLevel);
            height = Math.Max(height, 0);
            height *= height;
            return temperature - height * temperatureDecreasePerBlock;
        }

        public int GetPrecipitation(Index3 blockIndex)
        {
            int maxPrecipitation = 100;

            float rawValue = planet.BiomeGenerator.BiomeNoiseGenerator.GetTileableNoise2D(blockIndex.X, blockIndex.Y,
                Planet.Size.X * Chunk.CHUNKSIZE_X, Planet.Size.Y * Chunk.CHUNKSIZE_Y);

            int height = blockIndex.Z - planet.BiomeGenerator.SeaLevel;
            float precipitationDecreasePerBlock = 1;

            return (int)(((1 - rawValue) * maxPrecipitation) - (Math.Max(height, 0) * precipitationDecreasePerBlock));
        }
    }
}