using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedCottonBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.RedCotton;

        public override string Icon => "cotton_red";

        public override string[] Textures { get; } = { "cotton_red" };

        public override IMaterialDefinition Material { get; }

        public RedCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
