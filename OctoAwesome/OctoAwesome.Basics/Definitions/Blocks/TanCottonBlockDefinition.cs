using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class TanCottonBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.TanCotton;

        public override string Icon => "cotton_tan";

        public override string[] Textures { get; } = { "cotton_tan" };

        public override IMaterialDefinition Material { get; }

        public TanCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
