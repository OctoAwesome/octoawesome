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
            public PopulationHelper(int originX, int originY, int originZ, IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11)
            {
                this.originX = originX;
                this.originY = originY;
                this.originZ = originZ;

                this.column00 = column00;
                this.column01 = column01;
                this.column10 = column10;
                this.column11 = column11;
            }
            public static IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
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
                IChunkColumn column = getColumn(column00, column10, column01, column11, x, y);
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
                            if (i * i + j * j + k * k < radius * radius)
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

        private void PlantTree(PopulationHelper helper, int x, int y, int z, int height, int radius, ushort woodIndex, ushort leaveIndex)
        {
            helper.FillSphere(x, y, z + height, radius, leaveIndex);

            for (int i = 0; i < height + 2; i++)
            {
                helper.SetBlock(x, y, z + i, woodIndex);
            }
        }
        public void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11)
        {
            IBlockDefinition woodDefinition = blockDefinitions.FirstOrDefault(d => typeof(WoodBlockDefinition) == d.GetType());
            ushort woodIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), woodDefinition) + 1);
            IBlockDefinition leaveDefinition = blockDefinitions.FirstOrDefault(d => typeof(LeavesBlockDefinition) == d.GetType());
            ushort leaveIndex = (ushort)(Array.IndexOf(blockDefinitions.ToArray(), leaveDefinition) + 1);
            int treeCount = random.Next(4, 12);
            for (int i = 0; i < treeCount; i++)
            {
                int x = random.Next(Chunk.CHUNKSIZE_X / 2, Chunk.CHUNKSIZE_X * 3 / 2);
                int y = random.Next(Chunk.CHUNKSIZE_Y / 2, Chunk.CHUNKSIZE_Y * 3 / 2);

                IChunkColumn curColumn = PopulationHelper.getColumn(column00, column10, column01, column11, x, y);
                int z = curColumn.Heights[x % Chunk.CHUNKSIZE_X, y % Chunk.CHUNKSIZE_Y];
                if (z == -1)
                    continue;
                PopulationHelper helper = new PopulationHelper(x, y, z + 1, column00, column10, column01, column11);
                int height = random.Next(6, 16);
                PlantTree(helper, 0, 0, 0, height, random.Next(3, height - 2), woodIndex, leaveIndex);
            }
        }
    }
}
