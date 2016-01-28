using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class TreePopulator : IMapPopulator
    {
        private Random random = new Random();

        private int getTopBlockHeight(IChunkColumn column, int x, int y)
        {
            for (int z = column.Chunks.Length * Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
            {

                if (column.GetBlock(x, y, z) != 0)
                {
                    return z;
                }
            }
            return -1;
        }
        private IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11, int x, int y)
        {
            if (x >= Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                return column11;
            if (x < Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                return column01;
            if (x >= Chunk.CHUNKSIZE_X && y < Chunk.CHUNKSIZE_Y)
                return column10;
            return column00;
        }
        public void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            IBlockDefinition woodDefinition = blockDefinitions.FirstOrDefault(d => typeof(WoodBlockDefinition) == d.GetType());
            ushort woodIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), woodDefinition) + 1);
            int treeCount = random.Next(0, 8);
            for (int i = 0; i < treeCount; i++)
            {
                int x = random.Next(Chunk.CHUNKSIZE_X / 2, Chunk.CHUNKSIZE_X * 3 / 2);
                int y = random.Next(Chunk.CHUNKSIZE_Y / 2, Chunk.CHUNKSIZE_Y * 3 / 2);

                IChunkColumn curColumn = getColumn(column00, column01, column10, column11,x,y);
                int z = getTopBlockHeight(curColumn, x, y);
                if (z == -1)
                    continue;
                curColumn.SetBlock(x, y, z + 1, woodIndex);

            }
        }
    }
}
