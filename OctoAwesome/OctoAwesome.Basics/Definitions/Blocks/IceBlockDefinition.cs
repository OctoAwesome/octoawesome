using System;
using System.Drawing;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Ice; }
        }

        public override string Icon
        {
            get { return "ice"; }
        }


        public override string[] Textures { get; } = new[] { "ice" };

        public override IMaterialDefinition Material { get; }

        public IceBlockDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

    }
}
