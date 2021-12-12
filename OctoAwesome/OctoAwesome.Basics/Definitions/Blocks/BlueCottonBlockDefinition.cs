using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.BlueCotton;
        public override string Icon => "cotton_blue";
        public override string[] Textures { get; } = {"cotton_blue"};
        public override IMaterialDefinition Material { get; }
        public BlueCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

    }
}
