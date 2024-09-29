using OctoAwesome.Definitions;
using OctoAwesome.Location;
using System.Linq;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Base class for defining a specific tree type, used by the <see cref="TreePopulator"/>.
    /// </summary>
    public class TreeDefinition : ITreeDefinition
    {
        /// <inheritdoc />
        /// <remarks>This is <c>string.Empty</c> as trees need no names.</remarks>
        public string DisplayName => "";

        /// <inheritdoc />
        /// <remarks>This is <c>string.Empty</c> as trees need no icon.</remarks>
        public string Icon => "";

        /// <inheritdoc />
        public virtual int Order { get; init; }

        /// <inheritdoc />
        public virtual float MaxTemperature { get; init; }

        /// <inheritdoc />
        public virtual float MinTemperature { get; init; }

        /// <inheritdoc />
        public virtual int Density { get; init; }


    }
}
