using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for gravel blocks.
    /// </summary>
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Gravel;

        /// <inheritdoc />
        public override string Icon => "gravel";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "gravel" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GravelBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this gravel block definition.</param>
        public GravelBlockDefinition(GravelMaterialDefinition material)
        {
            Material = material;
        }
    }
}
