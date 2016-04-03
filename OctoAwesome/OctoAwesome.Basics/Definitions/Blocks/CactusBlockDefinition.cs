using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class CactusBlockDefinition : BlockDefinition
    {
        public override Bitmap Icon
        {
            get { return Textures[0]; }
        }

        public override string Name
        {
            get { return Languages.OctoBasics.Cactus; }
        }

        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                    (Bitmap)Image.FromFile("./Assets/OctoAwesome.Basics/Blocks/cactus_inside.png"),
                    (Bitmap)Image.FromFile("./Assets/OctoAwesome.Basics/Blocks/cactus_side.png"),
                    (Bitmap)Image.FromFile("./Assets/OctoAwesome.Basics/Blocks/cactus_top.png")
                };
            }
        }

        public override int GetTopTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
            }
        }

        public override int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    return 1;
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
            }
        }

        public override int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }
        }

        public override int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }

        }

        public override int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }

        }

        public override int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
            ushort topblock = manager.GetBlock(x, y, z + 1);

            switch (orientation)
            {
                case OrientationFlags.SideWest:
                case OrientationFlags.SideEast:
                    if (topblock != 0)
                        return 0;
                    else
                        return 2;
                case OrientationFlags.SideSouth:
                case OrientationFlags.SideNorth:
                case OrientationFlags.SideBottom:
                case OrientationFlags.SideTop:
                default:
                    return 1;
            }
        }

        public override int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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

        public override int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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

        public override int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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

        public override int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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

        public override int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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

        public override int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

            switch (orientation)
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
