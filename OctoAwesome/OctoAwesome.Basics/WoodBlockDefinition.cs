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
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetBottomTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetNorthTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetSouthTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 0;
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }

        }

        public int GetWestTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 0;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }

        }

        public int GetEastTextureIndex(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 0;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 1;
            }
        }

        public int GetTopTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 1;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetBottomTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 1;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetEastTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetWestTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                    return 1;
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetNorthTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 1;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
                default:
                    return 0;
            }
        }

        public int GetSouthTextureRotation(IBlock block)
        {
            switch (block.Orientation)
            {
                case OrientationFlags.SideNegativeX:
                case OrientationFlags.SidePositiveX:
                    return 1;
                case OrientationFlags.SideNegativeY:
                case OrientationFlags.SidePositiveY:
                case OrientationFlags.SideNegativeZ:
                case OrientationFlags.SidePositiveZ:
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
