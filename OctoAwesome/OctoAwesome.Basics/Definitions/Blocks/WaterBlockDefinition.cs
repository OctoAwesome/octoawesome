using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{

    public sealed class WaterBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Water;
        
        public override uint SolidWall => 0;
        
        public override string Icon => "water";
        
        public override string[] Textures { get; } = { "water" };
        
        public override IMaterialDefinition Material { get; }
        
        public WaterBlockDefinition(WaterMaterialDefinition material)
        {
            Material = material;
        }
    }
}
