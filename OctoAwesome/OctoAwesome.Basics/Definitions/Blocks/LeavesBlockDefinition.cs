using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for leave blocks.
    /// </summary>
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string Name => Languages.OctoBasics.Leaves;

        /// <inheritdoc />
        public override string Icon => "leaves";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "leaves" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeavesBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this leave block definition.</param>
        public LeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

    }
}
