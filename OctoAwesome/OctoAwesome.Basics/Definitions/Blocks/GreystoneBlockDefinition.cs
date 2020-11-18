using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreystoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Greystone; }
        }

        public override string Icon
        {
            get { return "greystone"; }
        }


        public override string[] Textures { get; } = new[] { "greystone" };

        public override IMaterialDefinition Material { get; }

        public GreystoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
