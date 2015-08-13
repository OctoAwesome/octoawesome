using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class WoodBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Wood"; }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/wood_top.png"); }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] {
                (Bitmap)Bitmap.FromFile("./Assets/wood_top.png"),
                (Bitmap)Bitmap.FromFile("./Assets/wood_side.png") };
            }
        }

        public PhysicalProperties GetProperties(IBlock block)
        {
            return new PhysicalProperties()
            {
                Density = 0.3f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public void Hit(IBlock block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }

        public int GetTopTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetBottomTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetNorthTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 0;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }
        }

        public int GetSouthTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 0;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }

        }

        public int GetWestTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 0;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }

        }

        public int GetEastTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 0;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }
        }

        public int GetTopTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 1;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetBottomTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 1;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetEastTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetWestTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetNorthTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 1;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public int GetSouthTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    return 1;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 0;
            }
        }

        public bool IsTopSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsBottomSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsNorthSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsSouthSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsWestSolidWall(IBlock block)
        {
            return true;
        }

        public bool IsEastSolidWall(IBlock block)
        {
            return true;
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new WoodBlock()
            {
                Orientation = orientation
            };
        }

        public Type GetBlockType()
        {
            return typeof(WoodBlock);
        }
    }
}
