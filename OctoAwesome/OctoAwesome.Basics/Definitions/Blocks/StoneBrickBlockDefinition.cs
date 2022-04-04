using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBrickBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.StoneBrick;

        public override string Icon => "brick_grey";

        public override string[] Textures { get; } = { "brick_grey", };

        public override IMaterialDefinition Material { get; }

        public StoneBrickBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
