using OctoAwesome.Serialization;

using System;

namespace OctoAwesome.Location
{
    /// <summary>
    /// Interface for universe implementations of OctoAwesome. A universe contains multiple planets and is a save state.
    /// </summary>
    public interface IUniverse : ISerializable
    {
        /// <summary>
        /// Gets the <see cref="Guid"/> for the universe.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the universe.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the random seed for the universe.
        /// </summary>
        int Seed { get; }
    }
}
