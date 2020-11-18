using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBrickBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.StoneBrick; }
        }

        public override string Icon
        {
            get { return "brick_grey"; }
        }


        public override string[] Textures { get; } =  new[] {"brick_grey",};

        public override IMaterialDefinition Material { get; }

        public StoneBrickBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
