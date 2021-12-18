using System;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Interface for objects identifiable by a <see cref="Guid"/>.
    /// </summary>
    public interface IIdentification
    {
        /// <summary>
        /// Gets the identifying <see cref="Guid"/> for this instance.
        /// </summary>
        Guid Id { get; }
    }
}
