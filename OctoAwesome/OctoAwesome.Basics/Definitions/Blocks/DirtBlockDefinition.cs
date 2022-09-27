using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for dirt blocks.
    /// </summary>
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Ground;

        /// <inheritdoc />
        public override string Icon => "dirt";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "dirt" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirtBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this dirt block definition.</param>
        public DirtBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }
    }
}
