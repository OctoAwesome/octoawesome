using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Leaves;
        
        public override string Icon => "leaves";
        
        public override string[] Textures { get; } = {"leaves"};
        
        public override IMaterialDefinition Material { get; }

        public LeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

    }
}
