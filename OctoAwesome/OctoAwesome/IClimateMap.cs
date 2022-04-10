namespace OctoAwesome
{
    /// <summary>
    /// Interface for the climate map of a planet.
    /// </summary>
    public interface IClimateMap
    {
        /// <summary>
        /// Gets the planet the climate map is for.
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Gets the temperature for a specific block position.
        /// </summary>
        /// <param name="blockIndex">The block index position to get the temperature at.</param>
        /// <returns>The temperature at the block position.</returns>
        float GetTemperature(Index3 blockIndex);

        /// <summary>
        /// Gets the precipitation for a specific block position.
        /// </summary>
        /// <param name="blockIndex">The block index position to get the precipitation at.</param>
        /// <returns>The precipitation value at the block position.</returns>
        int GetPrecipitation(Index3 blockIndex);
    }
}
