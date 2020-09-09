using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.RedCotton; }
        }

        public override string Icon
        {
            get { return "cotton_red"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_red"
                };
            }
        }

        public override IMaterialDefinition Material { get; }

        public RedCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }
    }
}
