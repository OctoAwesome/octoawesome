using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for brick blocks.
    /// </summary>
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Brick;

        /// <inheritdoc />
        public override string Icon => "brick_red";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "brick_red", };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrickBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this brick block definition.</param>
        public BrickBlockDefinition(BrickMaterialDefinition material)
        {
            Material = material;
        }

    }
}
