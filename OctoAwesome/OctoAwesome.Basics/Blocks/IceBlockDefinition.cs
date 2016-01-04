using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class IceBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return "Ice"; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap) Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/ice.png"); }
        }


        public override Bitmap[] Textures
        {
            get
            {
                return new[]
                {
                    (Bitmap) Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/ice.png")
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }
    }
}