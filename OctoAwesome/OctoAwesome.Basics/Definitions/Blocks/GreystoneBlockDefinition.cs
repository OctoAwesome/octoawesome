using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for grey-stone blocks.
    /// </summary>
    public sealed class GreystoneBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string Name => Languages.OctoBasics.Greystone;

        /// <inheritdoc />
        public override string Icon => "greystone";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "greystone" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GreystoneBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this stone block definition.</param>
        public GreystoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
