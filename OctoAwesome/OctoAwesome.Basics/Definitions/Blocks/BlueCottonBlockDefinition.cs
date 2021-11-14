using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BlueCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.BlueCotton; }
        }

        public override string Icon
        {
            get { return "cotton_blue"; }
        }


        public override string[] Textures { get; } = new[] {"cotton_blue"};

        public override IMaterialDefinition Material { get; }

        public BlueCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

    }
}
