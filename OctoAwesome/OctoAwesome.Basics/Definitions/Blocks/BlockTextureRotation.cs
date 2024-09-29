using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Blocks;
internal class BlockTextureRotation
{
    internal int Cactus(int lastValue, IDefinition def, Wall wall, ILocalChunkCache manager, int x, int y, int z)
    {

        OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);

        return wall switch
        {
            Wall.Top or Wall.Bottom or Wall.Back or Wall.Front => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast => 1,
                _ => 0,
            },
            Wall.Left or Wall.Right => orientation switch
            {
                OrientationFlags.SideSouth or OrientationFlags.SideNorth => 1,
                _ => 0,
            },
            _ => 0,
        };
    }
    internal int Wood(int lastValue, IDefinition def, Wall wall, ILocalChunkCache manager, int x, int y, int z)
    {
        OrientationFlags orientation = (OrientationFlags)manager.GetBlockMeta(x, y, z);
        return wall switch
        {
            Wall.Top or Wall.Bottom or Wall.Back or Wall.Front => orientation switch//top and bottom north south
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast => 1,
                _ => 0,
            },
            Wall.Left or Wall.Right => orientation switch //east west
            {
                OrientationFlags.SideSouth or OrientationFlags.SideNorth => 1,
                _ => 0,
            },
            _ => 0,//should never ever happen
        };
    }
}
