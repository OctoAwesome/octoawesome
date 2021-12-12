using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Stone;

        public override string Icon => "stone";

        public override string[] Textures { get; } = { "stone", };

        public override IMaterialDefinition Material { get; }

        public StoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
