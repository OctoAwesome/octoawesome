using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.RedPlank; }
        }

        public override string Icon
        {
            get { return "planks"; }
        }

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures
        {
            get
            {
                return new[] {
                "planks"};
            }
        }

        public override IMaterialDefinition Material { get; }

        public RedPlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }

    }
}
