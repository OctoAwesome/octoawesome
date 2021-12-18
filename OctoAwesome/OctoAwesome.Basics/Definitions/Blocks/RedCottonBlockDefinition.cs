using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for red cotton blocks.
    /// </summary>
    public sealed class RedCottonBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string Name => Languages.OctoBasics.RedCotton;

        /// <inheritdoc />
        public override string Icon => "cotton_red";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "cotton_red" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedCottonBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cotton block definition.</param>
        public RedCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
