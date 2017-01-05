using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class WaterBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Water; }
        }

        public override string Icon
        {
            get { return "water"; }
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


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "water","water_wave"
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
