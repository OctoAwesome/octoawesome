using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for orange leave blocks.
    /// </summary>
    public sealed class OrangeLeavesBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.OrangeLeaves;

        /// <inheritdoc />
        public override string Icon => "leaves_orange";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["leaves_orange"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrangeLeavesBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this leave block definition.</param>
        public OrangeLeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }
    }
}
