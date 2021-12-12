using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.RedPlank;

        public override string Icon => "planks_red";

        public override bool HasMetaData => true;

        public override string[] Textures { get; } = { "planks_red" };

        public override IMaterialDefinition Material { get; }

        public RedPlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

    }
}
