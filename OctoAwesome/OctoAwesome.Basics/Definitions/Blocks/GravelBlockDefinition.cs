using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Gravel;

        public override string Icon => "gravel";

        public override string[] Textures { get; } = { "gravel" };

        public override IMaterialDefinition Material { get; }

        public GravelBlockDefinition(GravelMaterialDefinition material)
        {
            Material = material;
        }
    }
}
