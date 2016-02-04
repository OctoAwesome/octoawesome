using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class TreePopulator : IMapPopulator
    {
        class PopulationHelper
        {
            private int originX, originY, originZ;
            IChunkColumn column00, column01, column10, column11;
            public PopulationHelper(int originX, int originY, int originZ, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
            {
                this.originX = originX;
                this.originY = originY;
                this.originZ = originZ;

                this.column00 = column00;
                this.column01 = column01;
                this.column10 = column10;
                this.column11 = column11;
            }
            public static IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11, int x, int y)
            {
                IChunkColumn column;
                if (x >= Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                    column = column11;
                else if (x < Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                    column = column01;
                else if (x >= Chunk.CHUNKSIZE_X && y < Chunk.CHUNKSIZE_Y)
                    column = column10;
                else
                    column = column00;


                return column;
            }
            public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
            {
                x += originX;
                y += originY;
                z += originZ;
                IChunkColumn column = getColumn(column00, column01, column10, column11, x, y);
                x %= Chunk.CHUNKSIZE_X;
                y %= Chunk.CHUNKSIZE_Y;
                column.SetBlock(x, y, z, block, meta);
            }
            public void FillSphere(int x, int y, int z, int radius, ushort block, int meta = 0)
            {
                for (int i = -radius; i <= radius; i++)
                {
                    for (int j = -radius; j <= radius; j++)
                    {
                        for (int k = -radius; k <= radius; k++)
                        {
                            if (i * i + j * j + k * k < 1.5f * radius)
                                SetBlock(x + i, y + j, z + k, block, meta);
                        }
                    }
                }
            }
            public ushort GetBlock(int x, int y, int z)
            {
                return 0;//TODO: implement
            }
        }

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
        private void PlantTree(PopulationHelper helper, int x, int y, int z, int height, int radius, ushort woodIndex, ushort leaveIndex)
        {
            helper.FillSphere(x, y, z + height, radius, leaveIndex);

            for (int i = 0; i < height + 2; i++)
            {
                helper.SetBlock(x, y, z + i, woodIndex);
            }
        }
        public void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            IBlockDefinition woodDefinition = blockDefinitions.FirstOrDefault(d => typeof(WoodBlockDefinition) == d.GetType());
            ushort woodIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), woodDefinition) + 1);
            IBlockDefinition leaveDefinition = blockDefinitions.FirstOrDefault(d => typeof(LeavesBlockDefinition) == d.GetType());
            ushort leaveIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), leaveDefinition) + 1);
            int treeCount = 1;//random.Next(1, 8);
            for (int i = 0; i < treeCount; i++)
            {
                int x = Chunk.CHUNKSIZE_X;//random.Next(Chunk.CHUNKSIZE_X / 2, Chunk.CHUNKSIZE_X * 3 / 2);
                int y = Chunk.CHUNKSIZE_Y - 1;// random.Next(Chunk.CHUNKSIZE_Y / 2, Chunk.CHUNKSIZE_Y * 3 / 2);

                IChunkColumn curColumn = PopulationHelper.getColumn(column00, column01, column10, column11, x, y);
                int z = getTopBlockHeight(curColumn, x, y);
                if (z == -1)
                    continue;
                PopulationHelper helper = new PopulationHelper(x, y, z + 1, column00, column01, column10, column11);
                //random.Next(4, 7), random.Next(3, 6)
                column00.SetBlock(31, y, z, woodIndex);
                column10.SetBlock(0, y, z, woodIndex);
                //helper.SetBlock(0, 0, 0, woodIndex);
                //helper.SetBlock(0, 1, 0, woodIndex);
                //helper.SetBlock(0,-1, 0, woodIndex);
                //helper.SetBlock(0, -2, 0, woodIndex);
                //PlantTree(helper, 0, 0, 0,6,3, woodIndex, leaveIndex);
                // PlantTree(helper, +1, 0, 0, 6, 3, woodIndex, leaveIndex);

            }
        }
    }
}
