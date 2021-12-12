using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Brick;
        public override string Icon => "brick_red";
        public override string[] Textures { get; } = {"brick_red",};
        public override IMaterialDefinition Material { get; }

        public BrickBlockDefinition(BrickMaterialDefinition material)
        {
            Material = material;
        }

    }
}
