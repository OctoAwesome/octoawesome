using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for red plank blocks.
    /// </summary>
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.RedPlank;

        /// <inheritdoc />
        public override string Icon => "planks_red";

        /// <inheritdoc />
        public override bool HasMetaData => true;

        /// <inheritdoc />
        public override string[] Textures { get; } = ["planks_red"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedPlankBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this wood plank block definition.</param>
        public RedPlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

    }
}
