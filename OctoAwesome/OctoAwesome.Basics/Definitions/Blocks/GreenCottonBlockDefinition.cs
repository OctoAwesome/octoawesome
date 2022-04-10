using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for green cotton blocks.
    /// </summary>
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string Name => Languages.OctoBasics.GreenCotton;

        /// <inheritdoc />
        public override string Icon => "cotton_green";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "cotton_green" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreenCottonBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cotton block definition.</param>
        public GreenCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

    }
}
