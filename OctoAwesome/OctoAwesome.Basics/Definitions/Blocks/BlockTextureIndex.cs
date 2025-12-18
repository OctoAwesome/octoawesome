using OctoAwesome.Chunking;
using OctoAwesome.Database;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;
using OctoAwesome.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Blocks;
internal class BlockTextureIndex
{

    internal int BatteryBlock(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        var meta = cache.GetBlockMeta(x, y, z);
        return meta & 7;
    }
  
    internal int Cactus(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        var meta = cache.GetBlockMeta(x, y, z);
        OrientationFlags orientation = (OrientationFlags)(meta & 0xFF);
        var rotData = (meta >> 7) & 2;

        if (rotData > 0)
            return rotData;
        ushort topblock = cache.GetBlock(x, y, z + 1);

        return wall switch
        {
            Wall.Top => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast or OrientationFlags.SideSouth or OrientationFlags.SideNorth => 1,
                _ => topblock != 0 ? 0 : 2,
            },
            Wall.Bottom => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast or OrientationFlags.SideSouth or OrientationFlags.SideNorth => 1,
                _ => topblock != 0 ? 0 : 2,
            },
            Wall.Front => orientation switch
            {
                OrientationFlags.SideSouth or OrientationFlags.SideNorth => topblock != 0 ? 0 : 2,
                _ => 1,
            },
            Wall.Back => orientation switch
            {
                OrientationFlags.SideSouth or OrientationFlags.SideNorth => topblock != 0 ? 0 : 2,
                _ => 1,
            },
            Wall.Left => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast => topblock != 0 ? 0 : 2,
                _ => 1,
            },
            Wall.Right => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast => topblock != 0 ? 0 : 2,
                _ => 1,
            },
            // Should never happen
            // Assert here
            _ => -1,
        };
    }
    internal int Grass(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        return wall switch
        {
            Wall.Top => 0,
            Wall.Bottom => 1,
            _ => 2
        };
    }
    internal int Light(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        var meta = cache.GetBlockMeta(x, y, z);
        return (meta & 1) == 1 ? 1 : 0;
    }
    internal int Red(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        static int RemoveEdgesOnAxis(ILocalChunkCache manager, Index3 centerBlock, Index3 axis, int sideOffset, ushort baseBlockIndex,
            int fullBlockTextureIndex)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                var otherIndex = manager.GetBlock(centerBlock + axis * i);

                if (otherIndex == baseBlockIndex)
                {
                    var bitOffset = (i + 1) + sideOffset;
                    fullBlockTextureIndex &= ~(1 << bitOffset);
                }
            }

            return fullBlockTextureIndex;
        }
        int fullBlockTextureIndex = 15;
        var centerBlock = new Index3(x, y, z);
        var baseBlockIndex = cache.GetBlock(centerBlock);

        var (firstAxis, secondAxis) = wall switch
        {
            Wall.Top => (Index3.UnitX, Index3.UnitY),
            Wall.Bottom => (-Index3.UnitX, Index3.UnitY),
            Wall.Left => (Index3.UnitY, -Index3.UnitZ),
            Wall.Right => (-Index3.UnitY, -Index3.UnitZ),
            Wall.Front => (Index3.UnitX, -Index3.UnitZ),
            Wall.Back => (-Index3.UnitX, -Index3.UnitZ),
            _ => throw new ArgumentOutOfRangeException(nameof(wall), wall, null)
        };

        fullBlockTextureIndex = RemoveEdgesOnAxis(cache, centerBlock, firstAxis, 0, baseBlockIndex, fullBlockTextureIndex);
        fullBlockTextureIndex = RemoveEdgesOnAxis(cache, centerBlock, secondAxis, 1, baseBlockIndex, fullBlockTextureIndex);

        return fullBlockTextureIndex;
    }
    internal int Snow(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        return wall switch
        {
            Wall.Top => 0,
            Wall.Bottom => 1,
            _ => 2
        };
    }

    internal int Wood(int lastResult, IDefinition def, Wall wall, ILocalChunkCache cache, int x, int y, int z)
    {
        OrientationFlags orientation = (OrientationFlags)cache.GetBlockMeta(x, y, z);

        return wall switch
        {
            Wall.Top or Wall.Bottom => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast or OrientationFlags.SideSouth or OrientationFlags.SideNorth => 1,
                _ => 0,
            },
            // North
            Wall.Back or Wall.Front => orientation switch
            {
                OrientationFlags.SideSouth or OrientationFlags.SideNorth => 0,
                _ => 1,
            },
            Wall.Left or Wall.Right => orientation switch
            {
                OrientationFlags.SideWest or OrientationFlags.SideEast => 0,
                _ => 1,
            },
            _ => -1,// Assert this
        };
    }

}
