using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for sand blocks.
    /// </summary>
    public sealed class SandBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Sand;

        /// <inheritdoc />
        public override string Icon => "sand";

        /// <inheritdoc />
        public override string[] Textures { get; init; } = ["sand"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SandBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this sand block definition.</param>
        public SandBlockDefinition(SandMaterialDefinition material)
        {
            Material = material;
        }
    }
}
