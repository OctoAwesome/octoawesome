using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class WaterBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return "Water"; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/water.png"); }
        }

        public override PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 0.3f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }


        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                    (Bitmap)Bitmap.FromFile("./Assets/water.png")
                };
            }
        }

        public override bool IsTopSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsBottomSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsNorthSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsSouthSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsWestSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsEastSolidWall(IPlanetResourceManager manager, int x, int y, int z)
        {
            return false;
        }
    }
}
