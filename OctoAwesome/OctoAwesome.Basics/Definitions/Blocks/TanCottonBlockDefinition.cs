using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for tan cotton blocks.
    /// </summary>
    public sealed class TanCottonBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.TanCotton;

        /// <inheritdoc />
        public override string Icon => "cotton_tan";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["cotton_tan"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TanCottonBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cotton block definition.</param>
        public TanCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
