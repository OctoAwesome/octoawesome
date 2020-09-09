using OctoAwesome.Information;
using System;
using System.Drawing;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Stone; }
        }

        public override string Icon
        {
            get { return "stone"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "stone",
                };
            }
        }

        public override IMaterialDefinition Material { get; }

        public StoneBlockDefinition(StoneMaterialDefinition material)
        {
            Material = material;
        }

    }
}
