using OctoAwesome.Information;
using OctoAwesome.Definitions;
using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Leaves; }
        }

        public override string Icon
        {
            get { return "leaves"; }
        }


        public override string[] Textures { get; } = new[] {"leaves"};

        public override IMaterialDefinition Material { get; }

        public LeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

    }
}
