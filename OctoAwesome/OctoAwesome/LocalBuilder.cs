using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class LocalBuilder
    {
        private int originX, originY, originZ;
        IChunkColumn column00, column01, column10, column11;
        public LocalBuilder(int originX, int originY, int originZ, IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11)
        {
            this.originX = originX;
            this.originY = originY;
            this.originZ = originZ;

            this.column00 = column00;
            this.column01 = column01;
            this.column10 = column10;
            this.column11 = column11;
        }

        private static IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
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
}
