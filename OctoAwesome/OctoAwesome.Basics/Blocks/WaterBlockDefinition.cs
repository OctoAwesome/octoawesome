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
            get { return (Bitmap) Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/water.png"); }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 1f,
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
                return new[]
                {
                    (Bitmap) Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/water.png")
                };
            }
        }

        public override bool IsTopSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsBottomSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsNorthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsSouthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsWestSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }

        public override bool IsEastSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return false;
        }
    }
}