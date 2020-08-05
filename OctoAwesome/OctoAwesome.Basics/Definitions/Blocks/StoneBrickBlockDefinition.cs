using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBrickBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.StoneBrick; }
        }

        public override string Icon
        {
            get { return "brick_grey"; }
        }


        public override string[] Textures { get; } =  new[] {"brick_grey",};

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }
    }
}
