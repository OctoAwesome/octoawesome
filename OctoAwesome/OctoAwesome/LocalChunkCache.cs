using OctoAwesome.Logging;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Chunk Cache für lokale Anwendungen.
    /// </summary>
    public class LocalChunkCache : ILocalChunkCache
    {
        private readonly LockSemaphore semaphore;
        private readonly LockSemaphore taskSemaphore;

        /// <summary>
        /// Aktueller Planet auf dem sich der Cache bezieht.
        /// </summary>
        public IPlanet Planet { get; }
        public Index2 CenterPosition { get; set; }

        /// <summary>
        /// Referenz auf den Globalen Cache
        /// </summary>
        private readonly IGlobalChunkCache globalCache;

        /// <summary>
        /// Die im lokalen Cache gespeicherten Chunks
        /// </summary>
        private readonly IChunkColumn?[] chunkColumns;
        private readonly ILogger logger;

        /// <summary>
        /// Größe des Caches in Zweierpotenzen
        /// </summary>
        private int limit;

        /// <summary>
        /// Maske, die die Grösse des Caches markiert
        /// </summary>
        private int mask;

        /// <summary>
        /// Gibt die Range in Chunks in alle Richtungen an (bsp. Range = 1 bedeutet centraler Block + links uns rechts jeweils 1 = 3)
        /// </summary>
        private int range;
        /// <summary>
        /// Task, der bei einem Wechsel des Zentralen Chunks neue nachlädt falls nötig
        /// </summary>
        private Task? loadingTask;

        /// <summary>
        /// Token, das angibt, ob der Chûnk-nachlade-Task abgebrochen werden soll
        /// </summary>
        private CancellationTokenSource? cancellationToken;
        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="globalCache">Referenz auf global Chunk Cache</param>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        /// <param name="range">Gibt die Range in alle Richtungen an.</param>
        public LocalChunkCache(IGlobalChunkCache globalCache, int dimensions, int range)
        {
            if (1 << dimensions < (range * 2) + 1)
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

        /// <summary>
        /// Setzt den Zentrums-Chunk für diesen lokalen Cache.
        /// </summary>
        /// <param name="planet">Der Planet, auf dem sich der Chunk befindet</param>
        /// <param name="index">Die Koordinaten an der sich der Chunk befindet</param>
        /// <param name="successCallback">Routine die Aufgerufen werden soll, falls das setzen erfolgreich war oder nicht</param>
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
                    loadingTask = loadingTask.ContinueWith(_ => InternalSetCenter(cancellationToken!.Token, index, successCallback));
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
        /// Interne Methode, in der der zentrale Chunk gesetzt wird. Die Chunks um den Zentrumschunk werden auch nachgeladen falls nötig
        /// </summary>
        /// <param name="token">Token, um zu prüfen, ob die aktualisierung abgeborchen werden soll</param>
        /// <param name="planet">Der Planet, auf dem die Chunks aktualisiert werden sollen</param>
        /// <param name="index">Der ins Zentrum zu setzende Chunk</param>
        /// <param name="successCallback">Routine die Aufgerufen werden soll, falls das setzen erfolgreich war oder nicht</param>
        private void InternalSetCenter(CancellationToken token, Index2 index, Action<bool>? successCallback)
        {
            if (Planet == null)
            {
                successCallback?.Invoke(true);
                return;
            }

            List<Index2> requiredChunkColumns = new List<Index2>();

            for (int x = -range; x <= range; x++)
            {
                for (int y = -range; y <= range; y++)
                {
                    Index2 local = new Index2(index.X + x, index.Y + y);
                    local.NormalizeXY(Planet.Size);
                    requiredChunkColumns.Add(local);
                }
            }

            // Erste Abbruchmöglichkeit
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

                // Zweite Abbruchmöglichkeit
                if (token.IsCancellationRequested)
                {
                    successCallback?.Invoke(false);
                    return;
                }

                using (semaphore.Wait())
                {
                    // Neuen Chunk laden
                    if (chunkColumn == null)
                    {
                        chunkColumn = globalCache.Subscribe(chunkColumnIndex);

                        if (chunkColumn?.Index != chunkColumnIndex)
                            logger.Error($"Loaded Chunk Index: {chunkColumn?.Index}, wanted: {chunkColumnIndex} ");
                        if (chunkColumns[flatIndex] != null)
                            logger.Error($"Chunk in Array!!: {flatIndex}, on index: {chunkColumns[flatIndex].Index} ");
                        chunkColumns[flatIndex] = chunkColumn;

                        if (chunkColumn == null)
                        {
                            successCallback?.Invoke(false);
                            return;
                        }
                    }
                }

                // Dritte Abbruchmöglichkeit
                if (token.IsCancellationRequested)
                {
                    successCallback?.Invoke(false);
                    return;
                }
            }

            successCallback?.Invoke(true);
        }


        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk? GetChunk(Index3 index)
            => GetChunk(index.X, index.Y, index.Z);

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="x">X Koordinate</param>
        /// <param name="y">Y Koordinate</param>
        /// <param name="z">Z Koordinate</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk? GetChunk(int x, int y, int z)
        {
            if (Planet == null || z < 0 || z >= Planet.Size.Z)
                return null;

            x = Index2.NormalizeAxis(x, Planet.Size.X);
            y = Index2.NormalizeAxis(y, Planet.Size.Y);

            IChunkColumn chunkColumn = chunkColumns[FlatIndex(x, y)];

            if (chunkColumn != null && chunkColumn.Index.X == x && chunkColumn.Index.Y == y)
                return chunkColumn.Chunks[z];

            return null;
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(Index3 index)
            => GetBlock(index.X, index.Y, index.Z);

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public BlockInfo GetBlockInfo(Index3 index)
        {
            var chunk = GetChunk(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ);

            if (chunk != null)
            {
                var flatIndex = Chunk.GetFlatIndex(index);
                var block = chunk.Blocks[flatIndex];
                var meta = chunk.MetaData[flatIndex];

                return new BlockInfo(index, block, meta);
            }

            return default;
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                return chunk.GetBlock(x, y, z);

            return 0;
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Die neue Block-ID.</param>
        public void SetBlock(Index3 index, ushort block)
            => SetBlock(index.X, index.Y, index.Z, block);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Die neue Block-ID</param>
        public void SetBlock(int x, int y, int z, ushort block)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            chunk?.SetBlock(x, y, z, block);
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            if (chunk != null)
                return chunk.GetBlockMeta(x, y, z);

            return 0;
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(Index3 index)
            => GetBlockMeta(index.X, index.Y, index.Z);

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">Die neuen Metadaten</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            var chunk = GetChunk(x >> Chunk.LimitX, y >> Chunk.LimitY, z >> Chunk.LimitZ);

            chunk?.SetBlockMeta(x, y, z, meta);
        }

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="meta">Die neuen Metadaten</param>
        public void SetBlockMeta(Index3 index, int meta)
            => SetBlockMeta(index.X, index.Y, index.Z, meta);

        /// <summary>
        /// Leert den Cache und gibt sie beim GlobalChunkCache wieder frei
        /// </summary>
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
        /// Gibt einen flachen Index um auf das Array <see cref="chunkColumns"/> zu zu greiffen
        /// </summary>
        /// <param name="x">Die X-Koordinate</param>
        /// <param name="y">Die Y-Koordinate</param>
        /// <returns>Der Abgeflachte index</returns>
        private int FlatIndex(int x, int y)
            => ((y & mask) << limit) | (x & mask);

        /// <summary>
        /// Returns the highest global z block position for the given global block position
        /// </summary>
        /// <param name="x">global x block position</param>
        /// <param name="y">global y block position</param>
        /// <returns>The highest global z position, or -1 if chunk was not loaded</returns>
        public int GroundLevel(int x, int y)
        {
            if (Planet == null)
                return -1;

            x = Index2.NormalizeAxis(x, Planet.Size.X);
            y = Index2.NormalizeAxis(y, Planet.Size.Y);

            IChunkColumn column = chunkColumns[FlatIndex(x >> Chunk.LimitX, y >> Chunk.LimitY)];

            if (column == null)
                return -1;

            return column.Heights[(x & (Chunk.CHUNKSIZE_X - 1)), (y & (Chunk.CHUNKSIZE_Y - 1))];
        }
    }
}
