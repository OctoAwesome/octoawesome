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
        {
            var list = new Dictionary<IChunkColumn, Dictionary<int, List<BlockInfo>>>();
            foreach (var block in blockInfos)
            {

                var x = block.Position.X + originX;
                var y = block.Position.Y + originY;
                var z = block.Position.Z + originZ;
                IChunkColumn column = GetColumn(column00, column10, column01, column11, x, y);
                var index = z / Chunk.CHUNKSIZE_Z;
                x %= Chunk.CHUNKSIZE_X;
                y %= Chunk.CHUNKSIZE_Y;
                z %= Chunk.CHUNKSIZE_Z;
                if (list.TryGetValue(column, out var columninfos))
                {
                    if (columninfos.TryGetValue(index, out var infos))
                        infos.Add((x, y, z, block.Block, block.Meta));
                    else
                        columninfos.Add(index, new List<BlockInfo> { (x, y, z, block.Block, block.Meta) });
                }
                else
                    list.Add(column, new Dictionary<int, List<BlockInfo>> { { index, new List<BlockInfo> { (x, y, z, block.Block, block.Meta) } } });
            }
            foreach (var item in list)
            {
                foreach (var item2 in item.Value)
                {
                    item.Key.Chunks[item2.Key].SetBlocks(item2.Value.ToArray());
                }
            }


            //foreach (var blockInfo in blockInfos.GroupBy(x => new Index2(x.Position.X + originX, x.Position.Y + originY)))
            //{
            //    IChunkColumn column = GetColumn(column00, column10, column01, column11, blockInfo.Key.X, blockInfo.Key.Y);
            //    foreach (var item in blockInfo
            //                            .GroupBy(x =>
            //                                        (x.Position.Z + originZ) / Chunk.CHUNKSIZE_Z)
            //                            .Select(x => (x.Key, x.Select(y =>
            //                                                      new BlockInfo(
            //                                                          y.Position.X % Chunk.CHUNKSIZE_X,
            //                                                          y.Position.Y % Chunk.CHUNKSIZE_Y,
            //                                                          (y.Position.Z + originZ) % Chunk.CHUNKSIZE_Z,
            //                                                          y.Block,
            //                                                          y.Meta)))))
            //    {
            //        column.Chunks[item.Key].SetBlocks(item.Item2.ToArray());
            //    }
            //}
        }

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
