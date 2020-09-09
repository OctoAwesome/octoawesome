using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class OrangeLeavesBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.OrangeLeaves; }
        }

        public override string Icon
        {
            get { return "leaves_orange"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "leaves_orange"
                };
            }
        }

        public override IMaterialDefinition Material { get; }

        public OrangeLeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }
    }
}
