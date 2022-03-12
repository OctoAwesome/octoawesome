﻿namespace OctoAwesome.Location
{
    /// <summary>
    /// Basisinterface für eine Klimakarte eines Planeten
    /// </summary>
    public interface IClimateMap
    {
        /// <summary>
        /// Der Planet für die ClimateMap
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Gibt die Temperatur für den Angegebenen Block zurück
        /// </summary>
        /// <param name="blockIndex">Die Block-Koordinate</param>
        /// <returns>Temperatur als <see cref="float"/></returns>
        float GetTemperature(Index3 blockIndex);

        /// <summary>
        /// Gibt den niederschlag für den angegebenen Block zurück
        /// </summary>
        /// <param name="blockIndex">Die Block-Koordinate</param>
        /// <returns>Den Niederschlagswert</returns>
        int GetPrecipitation(Index3 blockIndex);
    }
}
