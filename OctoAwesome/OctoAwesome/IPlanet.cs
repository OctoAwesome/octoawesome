using OctoAwesome.Serialization;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for all planet implementations.
    /// </summary>
    public interface IPlanet : ISerializable, IDisposable
    {
        /// <summary>
        /// Gets the id of the planet.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the <see cref="Guid"/> for the universe the planet resides in.
        /// </summary>
        Guid Universe { get; }

        /// <summary>
        /// Gets the random seed for this planet.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Gets the planet size in chunks.
        /// </summary>
        Index3 Size { get; }

        /// <summary>
        /// Gets the gravity on the planet.
        /// </summary>
        float Gravity { get; }

        /// <summary>
        /// Gets the climate map for the planet.
        /// </summary>
        IClimateMap ClimateMap { get; }

        /// <summary>
        /// Gets the map generator for the planet.
        /// </summary>
        IMapGenerator Generator { get; }

        /// <summary>
        /// Gets the global chunk cache for the planet.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}
