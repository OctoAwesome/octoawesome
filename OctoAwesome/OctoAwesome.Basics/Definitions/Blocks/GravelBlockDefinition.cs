using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Gravel; }
        }

        public override string Icon
        {
            get { return "gravel"; }
        }


        public override string[] Textures { get; } = new[] {"gravel"};

        public override IMaterialDefinition Material { get; }

        public GravelBlockDefinition(GravelMaterialDefinition material)
        {
            Material = material;
        }
    }
}
