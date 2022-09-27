using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for ice blocks.
    /// </summary>
    public sealed class IceBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Ice;

        /// <inheritdoc />
        public override string Icon => "ice";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "ice" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IceBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this ice block definition.</param>
        public IceBlockDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

    }
}
