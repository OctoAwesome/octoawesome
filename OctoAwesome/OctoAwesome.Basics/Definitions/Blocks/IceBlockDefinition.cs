using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Ice;
        
        public override string Icon => "ice";
        
        public override string[] Textures { get; } = { "ice" };
        
        public override IMaterialDefinition Material { get; }

        public IceBlockDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

    }
}
