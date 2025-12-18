using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for blue cotton blocks.
    /// </summary>
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.BlueCotton;

        /// <inheritdoc />
        public override string Icon => "cotton_blue";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["cotton_blue"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="BlueCottonBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cotton block definition.</param>
        public BlueCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

    }
}
