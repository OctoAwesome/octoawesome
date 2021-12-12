using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Sand;
        
        public override string Icon => "sand";
        
        public override string[] Textures { get; } = {"sand"};
        
        public override IMaterialDefinition Material { get; }

        public SandBlockDefinition(SandMaterialDefinition material)
        {
            Material = material;
        }
    }
}
