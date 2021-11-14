using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class DirtBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Ground; }
        }

        public override string Icon
        {
            get { return "dirt"; }
        }


        public override string[] Textures { get; } = new[] { "dirt" };



        public override IMaterialDefinition Material { get; }

        public DirtBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }
    }
}
