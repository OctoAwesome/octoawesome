using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for stone blocks.
    /// </summary>
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Stone;

        /// <inheritdoc />
        public override string Icon => "stone";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "stone", };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StoneBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this stone block definition.</param>
        public StoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
