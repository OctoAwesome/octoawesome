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

        public IEnumerable<Bitmap> Textures
        {
            get { return new[] { Resources.wood_bottom, Resources.wood_side }; }
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
