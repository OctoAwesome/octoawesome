using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class BrickBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return "Brick"; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/brick_red.png"); }
        }


        public override Bitmap[] Textures
        {
            get
            {
                return new[]
                {
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/brick_red.png"),
                };
            }
        }

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