using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for plank blocks.
    /// </summary>
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Plank;

        /// <inheritdoc />
        public override string Icon => "planks";

        /// <inheritdoc />
        public override bool HasMetaData => true;

        /// <inheritdoc />
        public override string[] Textures { get; } = ["planks"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlankBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this wood plank block definition.</param>
        public PlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }
    }
}
