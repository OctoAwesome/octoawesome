using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Logging;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Chunking
{
    /// <summary>
    /// Chunk Cache for local regions.
    /// </summary>
    public class LocalChunkCache : ILocalChunkCache
    {
        private readonly LockSemaphore semaphore;
        private readonly LockSemaphore taskSemaphore;

        /// <inheritdoc />
        public IPlanet Planet { get; }

        /// <summary>
        /// Gets or sets the center position of this local chunk cache.
        /// </summary>
        public Index2 CenterPosition { get; set; }

        /// <summary>
        /// Reference to the global chunk cache.
        /// </summary>
        private readonly IGlobalChunkCache globalCache;

        /// <summary>
        /// The chunks the local chunk cache manages.
        /// </summary>
        private readonly IChunkColumn?[] chunkColumns;
        private readonly ILogger logger;

        /// <summary>
        /// The cache limit in dualistic logarithmic scale.
        /// </summary>
        private int limit;

        /// <summary>
        /// Mask for the cache size.
        /// </summary>
        private int mask;

        /// <summary>
        /// The range in chunks in all directions (e.g. Range = 1 meaning central block + left and right 1 = 3)
        /// </summary>
        private int range;
        /// <summary>
        /// Task for loading additional chunks when necessary at center chunk change.
        /// </summary>
        private Task? loadingTask;

        /// <summary>
        /// Token for canceling the <see cref="loadingTask"/>.
        /// </summary>
        private CancellationTokenSource? cancellationToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalChunkCache"/> class.
        /// </summary>
        /// <param name="globalCache">Reference to the global cache.</param>
        /// <param name="dimensions">Dimensions of the local chunk cache in dualistic logarithmic scale.</param>
        /// <param name="range">The range of the chunk cache in all axis directions.</param>
        public LocalChunkCache(IGlobalChunkCache globalCache, int dimensions, int range)
        {
            if (1 << dimensions < range * 2 + 1)
                throw new ArgumentException("Range too big");


            semaphore = new LockSemaphore(1, 1);
            taskSemaphore = new LockSemaphore(1, 1);
            Planet = globalCache.Planet;
            this.globalCache = globalCache;
            this.range = range;

            limit = dimensions;
            mask = (1 << limit) - 1;
            chunkColumns = new IChunkColumn[(mask + 1) * (mask + 1)];
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(LocalChunkCache));
        }

        /// <inheritdoc />
        public bool SetCenter(Index2 index, Action<bool>? successCallback = null)
        {
            using (taskSemaphore.Wait())
            {
                var callerName = new StackFrame(1).GetMethod()?.Name;
                logger.Debug($"Set Center from {callerName}");
                CenterPosition = index;

                if (loadingTask != null && !loadingTask.IsCompleted)
                {
                    logger.Debug("Continue with task on index " + index);
                    loadingTask = loadingTask.ContinueWith(_ => InternalSetCenter(cancellationToken!.Token, index, successCallback), cancellationToken!.Token);
                }
                else
                {
                    logger.Debug("New task on index " + index);
                    cancellationToken?.Cancel();
                    cancellationToken?.Dispose();
                    cancellationToken = new CancellationTokenSource();
                    loadingTask = Task.Run(() => InternalSetCenter(cancellationToken.Token, index, successCallback));
                }
            }
            return true;
        }

        /// <summary>
        /// Internal method for setting the central chunk and load needed chunks in range asynchronously.
        /// </summary>
        /// <param name="token">The token to cancel the re-centering.</param>
        /// <param name="index">The new center to set the cache to.</param>
        /// <param name="successCallback">
        /// Action to call after re-centering happened with given success boolean.
        /// </param>
        private void InternalSetCenter(CancellationToken token, Index2 index, Action<bool>? successCallback)
        {
            try
            {
                var requiredChunkColumns = new List<Index2>();

                for (int x = -range; x <= range; x++)
                {
                    for (int y = -range; y <= range; y++)
                    {
                        Index2 local = new Index2(index.X + x, index.Y + y);
                        local.NormalizeXY(Planet.Size);
                        requiredChunkColumns.Add(local);
                    }
                }

                // First cancel opportunity
                if (token.IsCancellationRequested)
                {
                    successCallback?.Invoke(false);
                    return;
                }

                foreach (var chunkColumnIndex in requiredChunkColumns
                                                    .OrderBy(c => index.ShortestDistanceXY(c, new Index2(Planet.Size))
                                                    .LengthSquared()))
                {
                    int localX = chunkColumnIndex.X & mask;
                    int localY = chunkColumnIndex.Y & mask;
                    int flatIndex = FlatIndex(localX, localY);
                    var chunkColumn = chunkColumns[flatIndex];

                    // Remove old chunks if necessary

                    using (semaphore.Wait())
                    {
                        if (chunkColumn != null && chunkColumn.Index != chunkColumnIndex)
                        {
                            //logger.Debug($"Remove Chunk: {chunkColumn.Index}, new: {chunkColumnIndex}");
                            globalCache.Release(chunkColumn.Index);


                            chunkColumns[flatIndex] = null;
                            chunkColumn = null;
                        }
                    }

                    // Second cancel opportunity
                    if (token.IsCancellationRequested)
                    {
                        successCallback?.Invoke(false);
                        return;
                    }

                    using (semaphore.Wait())
                    {
                        // Load new chunk
                        if (chunkColumn == null)
                        {
                            chunkColumn = globalCache.Subscribe(chunkColumnIndex);

                            if (chunkColumn.Index != chunkColumnIndex)
                                logger.Error($"Loaded Chunk Index: {chunkColumn.Index}, wanted: {chunkColumnIndex} ");
                            var chunkInArray = chunkColumns[flatIndex];
                            if (chunkInArray != null)
                                logger.Error($"Chunk in Array!!: {flatIndex}, on index: {chunkInArray.Index} ");


                            chunkColumns[flatIndex] = chunkColumn;
                        }
                    }

                    // Third cancel opportunity
                    if (token.IsCancellationRequested)
                    {
                        successCallback?.Invoke(false);
                        return;
                    }
                }

                successCallback?.Invoke(true);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <inheritdoc />
        public IChunk? GetChunk(Index3 index)
            => GetChunk(index.X, index.Y, index.Z);

        /// <inheritdoc />
        public IChunk? GetChunk(int x, int y, int z)
        {
            if (z < 0 || z >= Planet.Size.Z)
                return null;

            x = Index2.NormalizeAxis(x, Planet.Size.X);
            y = Index2.NormalizeAxis(y, Planet.Size.Y);

            var chunkColumn = chunkColumns[FlatIndex(x, y)];

            if (chunkColumn != null && chunkColumn.Index.X == x && chunkColumn.Index.Y == y)
                return chunkColumn.Chunks[z];

            return null;
        }

        /// <inheritdoc />
        public ushort GetBlock(Index3 index)
            => GetBlock(index.X, index.Y, index.Z);

        /// <inheritdoc />
        public BlockInfo GetBlockInfo(Index3 index)
        {
            var chunk = GetChunk(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ);

            if (chunk != null)
            {
                var blockPlanetSize = Planet.Size * Chunk.CHUNKSIZE;
                index = (index + blockPlanetSize) % blockPlanetSize;
                var flatIndex = Chunk.GetFlatIndex(index);
                var block = chunk.Blocks[flatIndex];
                var meta = chunk.MetaData[flatIndex];

                return new BlockInfo(index, block, meta);
            }

            return default;
        }

        /// <inheritdoc />
        public ushort GetBlock(int x, int y, int z)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                return chunk.GetBlock(x, y, z);

            return 0;
        }

        /// <inheritdoc />
        public void SetBlock(Index3 index, ushort block)
            => SetBlock(index.X, index.Y, index.Z, block);

        /// <inheritdoc />
        public void SetBlock(int x, int y, int z, ushort block)
        {
            UpdateAdjacentChunks(x, y, z);
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            chunk?.SetBlock(x, y, z, block);
        }

        private void UpdateAdjacentChunks(int x, int y, int z)
        {
            var xBorder = x & (Chunk.CHUNKSIZE_X - 1);
            var yBorder = y & (Chunk.CHUNKSIZE_Y - 1);
            var zBorder = z & (Chunk.CHUNKSIZE_Z - 1);

            static int CalcDir(int v, int chunkLimit)
            {
                return ((v + 1) >> (chunkLimit - 1)) - 1;
            }
            
            if (xBorder is 0 or Chunk.CHUNKSIZE_X - 1)
            {
                var xDir = CalcDir(xBorder, Chunk.LimitX);
                // TODO: do not update if on border of chunk cache.
                GetChunk((x >> Chunk.LimitX) + xDir, y >> Chunk.LimitY, z >> Chunk.LimitZ)?.FlagDirty();
            }
            if (yBorder is 0 or Chunk.CHUNKSIZE_Y - 1)
            {
                var yDir = CalcDir(yBorder, Chunk.LimitX);
                // TODO: do not update if on border of chunk cache.
                GetChunk(x >> Chunk.LimitX, (y >> Chunk.LimitY) + yDir, z >> Chunk.LimitZ)?.FlagDirty();
            }
            if (zBorder is 0 or Chunk.CHUNKSIZE_Z - 1)
            {
                var zDir = CalcDir(zBorder, Chunk.LimitX);
                // TODO: do not update if on border of chunk cache.
                GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, (z >> Chunk.LimitZ) + zDir)?.FlagDirty();
            }
        }

        /// <inheritdoc />
        public int GetBlockMeta(int x, int y, int z)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                return chunk.GetBlockMeta(x, y, z);

            return 0;
        }

        /// <inheritdoc />
        public int GetBlockMeta(Index3 index)
            => GetBlockMeta(index.X, index.Y, index.Z);

        /// <inheritdoc />
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            UpdateAdjacentChunks(x, y, z);
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            chunk?.SetBlockMeta(x, y, z, meta);
        }

        /// <inheritdoc />
        public void SetBlockMeta(Index3 index, int meta)
            => SetBlockMeta(index.X, index.Y, index.Z, meta);

        /// <inheritdoc />
        public void Flush()
        {
            for (int i = 0; i < chunkColumns.Length; i++)
            {
                var chunkColumn = chunkColumns[i];
                if (chunkColumn == null)
                    continue;

                globalCache.Release(chunkColumn.Index);
                chunkColumns[i] = null;
            }
        }

        /// <summary>
        /// Gets a flattened index to access a <see cref="chunkColumns"/> item by x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate component.</param>
        /// <param name="y">The y coordinate component.</param>
        /// <returns>The flat array index.</returns>
        private int FlatIndex(int x, int y)
            => (((y & (mask)) << limit) | ((x & (mask))));

        /// <inheritdoc />
        public int GroundLevel(int x, int y)
        {
            x = Index2.NormalizeAxis(x, Planet.Size.X);
            y = Index2.NormalizeAxis(y, Planet.Size.Y);

            var column = chunkColumns[FlatIndex(x >> Chunk.LimitX, y >> Chunk.LimitY)];

            if (column == null)
                return -1;

            return column.Heights[(x & (Chunk.CHUNKSIZE_X - 1)), (y & (Chunk.CHUNKSIZE_Y - 1))];
        }
}
}
