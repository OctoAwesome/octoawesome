using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return "Stone"; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/stone.png"); }
        }


        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                    (Bitmap)Bitmap.FromFile("./Assets/stone.png"),
                };
            }
        }

        public override PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 0.9f,
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
