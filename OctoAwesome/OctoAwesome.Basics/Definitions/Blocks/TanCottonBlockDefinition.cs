using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class TanCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.TanCotton; }
        }

        public override string Icon
        {
            get { return "cotton_tan"; }
        }


        public override string[] Textures { get; } = new[] {"cotton_tan"};

        public override IMaterialDefinition Material { get; }

        public TanCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
