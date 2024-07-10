using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for stone brick blocks.
    /// </summary>
    public sealed class StoneBrickBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.StoneBrick;

        /// <inheritdoc />
        public override string Icon => "brick_grey";

        /// <inheritdoc />
        public override string[] Textures { get; init; } = ["brick_grey",];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoneBrickBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this stone block definition.</param>
        public StoneBrickBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
