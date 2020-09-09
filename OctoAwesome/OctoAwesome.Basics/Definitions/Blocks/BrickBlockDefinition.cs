using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Brick; }
        }

        public override string Icon
        {
            get { return "brick_red"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "brick_red",
                };
            }
        }

        public override IMaterialDefinition Material { get; }

        public BrickBlockDefinition(BrickMaterialDefinition material)
        {
            Material = material;
        }

    }
}
