using OctoAwesome.Definitions;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Base class for defining a specific tree type, used by the <see cref="TreePopulator"/>.
    /// </summary>
    public abstract class TreeDefinition : ITreeDefinition
    {
        /// <inheritdoc />
        /// <remarks>This is <c>string.Empty</c> as trees need no names.</remarks>
        public string Name => "";

        /// <inheritdoc />
        /// <remarks>This is <c>string.Empty</c> as trees need no icon.</remarks>
        public string Icon => "";

        /// <inheritdoc />
        public abstract int Order { get; }

        /// <inheritdoc />
        public abstract float MaxTemperature { get; }

        /// <inheritdoc />
        public abstract float MinTemperature { get; }

        /// <inheritdoc />
        public abstract int GetDensity(IPlanet planet, Index3 index);

        /// <inheritdoc />
        public abstract void Init(IDefinitionManager definitionManager);

        /// <inheritdoc />
        public abstract void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
