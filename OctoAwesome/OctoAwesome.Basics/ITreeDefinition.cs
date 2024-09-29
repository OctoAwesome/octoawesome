using OctoAwesome.Definitions;
using OctoAwesome.Location;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Interface for defining a specific tree type, used by the <see cref="TreePopulator"/>.
    /// </summary>
    public interface ITreeDefinition : IDefinition
    {
        /// <summary>
        /// Gets a value indicating the priority order in which the trees should be tried to be planted.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Gets the maximum temperature the tree type can grow in.
        /// </summary>
        float MaxTemperature { get; }

        /// <summary>
        /// Gets the minimal temperature the tree type can grow in.
        /// </summary>
        float MinTemperature { get; }
        /// <summary>
        /// Gets the number of trees that should be tried to be planted in a specific chunk.
        /// </summary>
        int Density { get; }
    }
}
