using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class SandBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Sand; }
        }

        public override string Icon
        {
            get { return "sand"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "sand"
                };
            }
        }

        public override IMaterialDefinition Material { get; }

        public SandBlockDefinition(SandMaterialDefinition material)
        {
            Material = material;
        }
    }
}
