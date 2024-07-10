using OctoAwesome.Definitions;
using OctoAwesome.Location;
using System.Linq;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Base class for defining a specific tree type, used by the <see cref="TreePopulator"/>.
    /// </summary>
    public  class TreeDefinition : ITreeDefinition
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
        public virtual int GetDensity(IPlanet planet, Index3 index)
            => 0;

        /// <inheritdoc />
        public virtual void Init(IDefinitionManager definitionManager) { }

        /// <inheritdoc />
        public virtual void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed) { }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
    }
}
