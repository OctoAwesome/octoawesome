using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for a list of entities.
    /// </summary>
    public interface IEntityList : ICollection<Entity>
    {
        /// <summary>
        /// Gets an enumeration of <see cref="FailEntityChunkArgs"/>
        /// depicting entities that are not part of this chunk anymore.
        /// </summary>
        /// <returns>The entities that are not part of this chunk anymore.</returns>
        IEnumerable<FailEntityChunkArgs> FailChunkEntity();
    }
}
