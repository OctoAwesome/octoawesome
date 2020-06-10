using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Erzeugt ein lokales Koordinatensystem.
    /// </summary>
    public class LocalBuilder
    {
        private readonly int originX, originY, originZ;

        private readonly IChunkColumn column00, column01, column10, column11;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse LocalBuilder
        /// </summary>
        /// <param name="originX"></param>
        /// <param name="originY"></param>
        /// <param name="originZ"></param>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static IChunkColumn GetColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column00"></param>
        /// <param name="column10"></param>
        /// <param name="column01"></param>
        /// <param name="column11"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetSurfaceHeight(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
        {
            IChunkColumn curColumn = GetColumn(column00, column10, column01, column11, x, y);
            return curColumn.Heights[x % Chunk.CHUNKSIZE_X, y % Chunk.CHUNKSIZE_Y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="block"></param>
        /// <param name="meta"></param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            x += originX;
            y += originY;
            z += originZ;
            IChunkColumn column = GetColumn(column00, column10, column01, column11, x, y);
            var index = z / Chunk.CHUNKSIZE_Z;
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            z %= Chunk.CHUNKSIZE_Z;
            var flatIndex = Chunk.GetFlatIndex(x, y, z);
            column.Chunks[index].Blocks[flatIndex] = block;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockInfos"></param>
        public void SetBlocks(params BlockInfo[] blockInfos)
            => blockInfos
                    .Select(b =>
                    {
                        var x = b.Position.X + originX;
                        var y = b.Position.Y + originY;
                        var z = b.Position.Z + originZ;
                        IChunkColumn column = GetColumn(column00, column10, column01, column11, x, y);
                        var index = z / Chunk.CHUNKSIZE_Z;
                        x %= Chunk.CHUNKSIZE_X;
                        y %= Chunk.CHUNKSIZE_Y;
                        z %= Chunk.CHUNKSIZE_Z;
                        var info = new BlockInfo(x, y, z, b.Block, b.Meta);
                        return new { info, index, column };
                    })
                    .GroupBy(a => a.column)
                    .ForEach(column => column
                        .GroupBy(i => i.index)
                        .ForEach(i => column.Key.Chunks[i.Key].SetBlocks(i.Select(b => b.info).ToArray())));
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="radius"></param>
        /// <param name="block"></param>
        /// <param name="meta"></param>
        public void FillSphere(int x, int y, int z, int radius, ushort block, int meta = 0)
        {
            var blockInfos = new List<BlockInfo>(radius * 6);

            for (var i = -radius; i <= radius; i++)
            {
                for (var j = -radius; j <= radius; j++)
                {
                    for (var k = -radius; k <= radius; k++)
                    {
                        if (i * i + j * j + k * k < radius * radius)
                            blockInfos.Add((x + i, y + j, z + k, block, meta));
                    }
                }
            }
            SetBlocks(blockInfos.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public ushort GetBlock(int x, int y, int z)
        {
            x += originX;
            y += originY;
            z += originZ;
            IChunkColumn column = GetColumn(column00, column10, column01, column11, x, y);
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            return column.GetBlock(x, y, z);
        }
    }
}
