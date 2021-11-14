using OctoAwesome.Basics.Properties;
using OctoAwesome.Information;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class PlankBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Plank; }
        }

        public override string Icon
        {
            get { return "planks_red"; }
        }

        public override bool HasMetaData { get { return true; } }

        public override string[] Textures { get; } = new[] {"planks_red"};

        public override IMaterialDefinition Material { get; }

        public PlankBlockDefinition(WoodMaterialDefinition material)
        {
            Material = material;
        }
    }
}
