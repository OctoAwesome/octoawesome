using OctoAwesome.Database;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for database provider.
    /// </summary>
    public interface IDatabaseProvider
    {
        /// <summary>
        /// Gets a database for a specific identifying tag type.
        /// </summary>
        /// <param name="fixedValueSize">Whether the database should have fixed value size.</param>
        /// <typeparam name="T">The type of the identifying tag.</typeparam>
        /// <returns>The database for the specific identifying tag type.</returns>
        Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new();

        /// <summary>
        /// Gets a database for a specific universe with an identifying tag type.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> for the universe.</param>
        /// <param name="fixedValueSize">Whether the database should have fixed value size.</param>
        /// <typeparam name="T">The type of the identifying tag.</typeparam>
        /// <returns>The database for the specific identifying tag type.</returns>
        Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new();

        /// <summary>
        /// Gets a database for a specific universe and planet with an identifying tag type.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> for the universe.</param>
        /// <param name="planetId">The id of the planet.</param>
        /// <param name="fixedValueSize">Whether the database should have fixed value size.</param>
        /// <typeparam name="T">The type of the identifying tag.</typeparam>
        /// <returns>The database for the specific identifying tag type.</returns>
        Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new();
    }
}