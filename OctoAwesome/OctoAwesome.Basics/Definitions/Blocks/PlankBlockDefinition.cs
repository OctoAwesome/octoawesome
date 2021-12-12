using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Plank;
        
        public override string Icon => "planks";
        
        public override bool HasMetaData => true;
        
        public override string[] Textures { get; } = {"planks"};
        
        public override IMaterialDefinition Material { get; }

        public PlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }
    }
}
