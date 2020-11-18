using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.GreenCotton; }
        }

        public override string Icon
        {
            get { return "cotton_green"; }
        }


        public override string[] Textures { get; } =  new[] {"cotton_green"};

        public override IMaterialDefinition Material { get; }

        public GreenCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

    }
}
