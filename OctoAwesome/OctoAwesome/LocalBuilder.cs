using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// A builder for managing 2x2 chunk columns relative to a given origin coordinate system.
    /// </summary>
    public class LocalBuilder
    {
        private readonly int originX, originY, originZ;

        private readonly IChunkColumn column00, column01, column10, column11;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalBuilder"/> class.
        /// </summary>
        /// <param name="originX">The x component of the origin of the local coordinate system.</param>
        /// <param name="originY">The y component of the origin of the local coordinate system.</param>
        /// <param name="originZ">The z component of the origin of the local coordinate system.</param>
        /// <param name="column00">The chunk at chunk index (0, 0).</param>
        /// <param name="column10">The chunk at chunk index (1, 0).</param>
        /// <param name="column01">The chunk at chunk index (0, 1).</param>
        /// <param name="column11">The chunk at chunk index (1, 1).</param>
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
        /// Select a chunk column by a coordinate in relative coordinates to <see cref="column00"/>.
        /// </summary>
        /// <param name="column00">The chunk at chunk index (0, 0).</param>
        /// <param name="column10">The chunk at chunk index (1, 0).</param>
        /// <param name="column01">The chunk at chunk index (0, 1).</param>
        /// <param name="column11">The chunk at chunk index (1, 1).</param>
        /// <param name="x">The x component to get the chunk at(In block coordinates).</param>
        /// <param name="y">The y component to get the chunk at(In block coordinates).</param>
        /// <returns>The chunk column the given coordinate contains; <c>null</c> if out of range.</returns>
        public static IChunkColumn? GetColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
        {
            IChunkColumn? column = null;

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
        /// Gets the surface height at a coordinate in relative coordinates to <see cref="column00"/>.
        /// </summary>
        /// <param name="column00">The chunk at chunk index (0, 0).</param>
        /// <param name="column10">The chunk at chunk index (1, 0).</param>
        /// <param name="column01">The chunk at chunk index (0, 1).</param>
        /// <param name="column11">The chunk at chunk index (1, 1).</param>
        /// <param name="x">The x component of the coordinate to get the surface height at.</param>
        /// <param name="y">The y component of the coordinate to get the surface height at.</param>
        /// <returns>The surface height at the given coordinate; <c>-1</c> if out of range.</returns>
        public static int GetSurfaceHeight(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
        {
            var curColumn = GetColumn(column00, column10, column01, column11, x, y);
            if (curColumn == null)
                return -1;
            return curColumn.Heights[x % Chunk.CHUNKSIZE_X, y % Chunk.CHUNKSIZE_Y];
        }

        /// <summary>
        /// Set a block at a coordinate relative to <see cref="column00"/>.
        /// </summary>
        /// <param name="x">The x component of the coordinate to set the block at.</param>
        /// <param name="y">The y component of the coordinate to set the block at.</param>
        /// <param name="z">The z component of the coordinate to set the block at.</param>
        /// <param name="block">The block type id to set the block to.</param>
        /// <param name="meta">The meta data to set the block to.</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            x += originX;
            y += originY;
            z += originZ;
            var column = GetColumn(column00, column10, column01, column11, x, y);

            Debug.Assert(column != null, nameof(column) + " != null");
            var index = z / Chunk.CHUNKSIZE_Z;
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            z %= Chunk.CHUNKSIZE_Z;
            var flatIndex = Chunk.GetFlatIndex(x, y, z);
            column.Chunks[index].Blocks[flatIndex] = block;
        }

        /// <summary>
        /// Sets multiple blocks relative to <see cref="column00"/>.
        /// </summary>
        /// <param name="issueNotification">A value indicating whether the block changes should be notified.</param>
        /// <param name="blockInfos">The blocks to set.</param>
        public void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos)
            => blockInfos
                    .Select(b =>
                    {
                        var x = b.Position.X + originX;
                        var y = b.Position.Y + originY;
                        var z = b.Position.Z + originZ;
                        var column = GetColumn(column00, column10, column01, column11, x, y);

                        Debug.Assert(column != null, nameof(column) + " != null");
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
                        .ForEach(i => column.Key.Chunks[i.Key].SetBlocks(issueNotification, i.Select(b => b.info).ToArray())));

        /// <summary>
        /// Fills a sphere centered around coordinate relative to <see cref="column00"/>.
        /// </summary>
        /// <param name="x">The x component of the center coordinate to.</param>
        /// <param name="y">The y component of the center coordinate to.</param>
        /// <param name="z">The z component of the center coordinate to.</param>
        /// <param name="radius">The radius of the sphere to fill.</param>
        /// <param name="block">The block type id to fill the sphere with.</param>
        /// <param name="meta">The meta data to fill the sphere with.</param>
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
            SetBlocks(false, blockInfos.ToArray());
        }

        /// <summary>
        /// Gets the block id at a coordinate in relative coordinates to <see cref="column00"/>.
        /// </summary>
        /// <param name="x">The x component of the coordinate to get the block id at.</param>
        /// <param name="y">The y component of the coordinate to get the block id at.</param>
        /// <param name="z">The z component of the coordinate to get the block id at.</param>
        /// <returns>The block id.</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            x += originX;
            y += originY;
            z += originZ;
            var column = GetColumn(column00, column10, column01, column11, x, y);
            Debug.Assert(column != null, nameof(column) + " != null");
            x %= Chunk.CHUNKSIZE_X;
            y %= Chunk.CHUNKSIZE_Y;
            return column.GetBlock(x, y, z);
        }
    }
}
