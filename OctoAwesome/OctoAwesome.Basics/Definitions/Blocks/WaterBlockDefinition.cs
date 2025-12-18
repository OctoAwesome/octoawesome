using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for water blocks.
    /// </summary>
    public sealed class WaterBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Water;

        /// <inheritdoc />
        public override uint SolidWall => 0;

        /// <inheritdoc />
        public override string Icon => "water";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["water"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaterBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this water block definition.</param>
        public WaterBlockDefinition(WaterMaterialDefinition material)
        {
            Material = material;
        }
    }
}
