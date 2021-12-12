using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Ground;

        public override string Icon => "dirt";

        public override string[] Textures { get; } = { "dirt" };

        public override IMaterialDefinition Material { get; }

        public DirtBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }
    }
}
