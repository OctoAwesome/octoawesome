using OctoAwesome.Information;
using OctoAwesome.Location;

using System;

namespace OctoAwesome.Chunking
{
    /// <summary>
    /// Interface for a chunk cache that manages a local region.
    /// </summary>
    public interface ILocalChunkCache
    {
        /// <summary>
        /// Sets the center of this chunk.
        /// </summary>
        /// <param name="index">The new center for the cache.</param>
        /// <param name="successCallback">The action to be called</param>
        bool SetCenter(Index2 index, Action<bool>? successCallback = null);

        /// <summary>
        /// Gets the planet the local chunk cache uses.
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Gets the chunk at the given chunk index coordinates.
        /// </summary>
        /// <param name="index">The chunk index to retrieve the chunk for.</param>
        /// <returns>The retrieved chunk;<c>null</c> if chunk is out of cache range.</returns>
        /// <seealso cref="GetChunk(int,int,int)"/>
        IChunk? GetChunk(Index3 index);

        /// <summary>
        /// Gets the chunk at the given chunk index coordinates.
        /// </summary>
        /// <param name="x">The chunk index x-component to retrieve the chunk for.</param>
        /// <param name="y">The chunk index y-component to retrieve the chunk for.</param>
        /// <param name="z">The chunk index z-component to retrieve the chunk for.</param>
        /// <returns>The retrieved chunk;<c>null</c> if chunk is out of cache range.</returns>
        /// <seealso cref="GetChunk(OctoAwesome.Location.Index3)"/>
        IChunk? GetChunk(int x, int y, int z);

        /// <summary>
        /// Clears the cache and releases the chunk columns in the <see cref="IGlobalChunkCache"/>.
        /// </summary>
        void Flush();

        /// <summary>
        /// Gets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to retrieve the block id for.</param>
        /// <returns>The block id at the given block coordinates; defaults to <c>0</c> if out of cache range.</returns>
        ushort GetBlock(Index3 index);

        /// <summary>
        /// Gets the block info for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to retrieve the block id for.</param>
        /// <returns>The block info at the given block coordinates; <c>default(BlockInfo)</c> if out of cache range.</returns>
        BlockInfo GetBlockInfo(Index3 index);

        /// <summary>
        /// Gets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to retrieve the block id for.</param>
        /// <param name="y">The block index y-component to retrieve the block id for.</param>
        /// <param name="z">The block index z-component to retrieve the block id for.</param>
        /// <returns>The block id at the given block coordinates; defaults to <c>0</c> if out of cache range.</returns>
        ushort GetBlock(int x, int y, int z);

        /// <summary>
        /// Sets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to set the new block id for.</param>
        /// <param name="block">The new block id to set the block to.</param>
        /// <remarks>Does nothing if <paramref name="index"/> is out of cache range.</remarks>
        void SetBlock(Index3 index, ushort block);

        /// <summary>
        /// Sets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block id for.</param>
        /// <param name="y">The block index y-component to set the new block id for.</param>
        /// <param name="z">The block index z-component to set the new block id for.</param>
        /// <param name="block">The new block id to set the block to.</param>
        /// <remarks>Does nothing if block index is out of cache range.</remarks>
        void SetBlock(int x, int y, int z, ushort block);

        /// <summary>
        /// Gets the block meta info for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to retrieve the block meta info for.</param>
        /// <param name="y">The block index y-component to retrieve the block meta info for.</param>
        /// <param name="z">The block index z-component to retrieve the block meta info for.</param>
        /// <returns>
        /// The block meta info at the given block coordinates; defaults to <c>0</c> if out of cache range.
        /// </returns>
        int GetBlockMeta(int x, int y, int z);

        /// <summary>
        /// Gets the block meta info for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to retrieve the block meta info for.</param>
        /// <returns>
        /// The block meta info at the given block coordinates; defaults to <c>0</c> if out of cache range.
        /// </returns>
        int GetBlockMeta(Index3 index);

        /// <summary>
        /// Sets the block meta info for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block meta info for.</param>
        /// <param name="y">The block index y-component to set the new block meta info for.</param>
        /// <param name="z">The block index z-component to set the new block meta info for.</param>
        /// <param name="meta">The new block meta info to set the block to.</param>
        /// <remarks>Does nothing if block index is out of cache range.</remarks>
        void SetBlockMeta(int x, int y, int z, int meta);

        /// <summary>
        /// Sets the block meta info for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to set the new block meta info for.</param>
        /// <param name="meta">The new block meta info to set the block to.</param>
        /// <remarks>Does nothing if <paramref name="index"/> is out of cache range.</remarks>
        void SetBlockMeta(Index3 index, int meta);

        /// <summary>
        /// Gets the block ground level at the given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block meta info for.</param>
        /// <param name="y">The block index y-component to set the new block meta info for.</param>
        /// <returns>
        /// The block ground level at the given block index coordinate; or <c>-1</c> if the corresponding chunk is not loaded.
        /// </returns>
        int GroundLevel(int x, int y);
    }
}