using System;

namespace OctoAwesome.Location
{
    /// <summary>
    /// A position of a chunk on a planet.
    /// </summary>
    public struct PlanetIndex3 : IEquatable<PlanetIndex3>
    {
        /// <summary>
        /// The planet id.
        /// </summary>
        public int Planet;

        /// <summary>
        /// The chunk position.
        /// </summary>
        public Index3 ChunkIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetIndex3"/> struct.
        /// </summary>
        /// <param name="planet">The id of the planet.</param>
        /// <param name="chunkIndex">The <see cref="Index3"/> position of the chunk.</param>
        public PlanetIndex3(int planet, Index3 chunkIndex)
        {
            Planet = planet;
            ChunkIndex = chunkIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetIndex3"/> struct.
        /// </summary>
        /// <param name="planet">The id of the planet</param>
        /// <param name="x">The X component of the chunk position.</param>
        /// <param name="y">The Y component of the chunk position.</param>
        /// <param name="z">The Z component of the chunk position.</param>
        public PlanetIndex3(int planet, int x, int y, int z) : this(planet, new Index3(x, y, z)) { }

        /// <summary>
        /// Compares whether two <see cref="PlanetIndex3"/> reference the same position.
        /// </summary>
        /// <param name="i1">The first <see cref="PlanetIndex3"/> to compare with.</param>
        /// <param name="i2">The second <see cref="PlanetIndex3"/> to compare to.</param>
        /// <returns>Whether the two positions are equal.</returns>
        public static bool operator ==(PlanetIndex3 i1, PlanetIndex3 i2)
            => i1.Equals(i2);

        /// <summary>
        /// Compares whether two <see cref="PlanetIndex3"/> do not reference the same position.
        /// </summary>
        /// <param name="i1">The first <see cref="PlanetIndex3"/> to compare with.</param>
        /// <param name="i2">The second <see cref="PlanetIndex3"/> to compare to.</param>
        /// <returns>Whether the two positions are unequal.</returns>
        public static bool operator !=(PlanetIndex3 i1, PlanetIndex3 i2)
            => !i1.Equals(i2);

        /// <inheritdoc />
        public bool Equals(PlanetIndex3 other)
        {
            return Planet == other.Planet && ChunkIndex.Equals(other.ChunkIndex);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is PlanetIndex3 other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Planet, ChunkIndex);
        }
    }
}
