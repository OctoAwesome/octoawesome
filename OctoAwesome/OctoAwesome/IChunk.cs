using OctoAwesome.Pooling;

using System;
using OctoAwesome.Notifications;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for chunks.
    /// </summary>
    public interface IChunk : IPoolElement
    {
        /// <summary>
        /// Gets the planet the chunk is part of.
        /// </summary>
        int PlanetId { get; }

        /// <summary>
        /// Gets the index of the chunk.
        /// </summary>
        Index3 Index { get; }

        /// <summary>
        /// Gets an array depicting all block ids in the chunk.
        /// </summary>
        /// <remarks>Flat indexing is the same as with <see cref="MetaData"/>.</remarks>
        ushort[] Blocks { get; }

        /// <summary>
        /// Gets an array depicting all block metadata in the chunk.
        /// </summary>
        /// <remarks>Flat indexing is the same as with <see cref="MetaData"/>.</remarks>
        int[] MetaData { get; }

        /// <summary>
        /// Gets or sets the version of this chunk. Indicating changes of the chunk content.
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// Gets the block id for a given block index coordinate local to the chunk
        /// </summary>
        /// <param name="index">The block index to retrieve the block id for(in local block coordinates).</param>
        /// <returns>The block id at the given block coordinates; wraps around if out of range.</returns>
        ushort GetBlock(Index3 index);

        /// <summary>
        /// Gets the block id for a given block index coordinate local to the chunk.
        /// </summary>
        /// <param name="x">The block index x-component to retrieve the block id for(in local block coordinates).</param>
        /// <param name="y">The block index y-component to retrieve the block id for(in local block coordinates).</param>
        /// <param name="z">The block index z-component to retrieve the block id for(in local block coordinates).</param>
        /// <returns>The block id at the given block coordinates; wraps around if out of range.</returns>
        ushort GetBlock(int x, int y, int z);

        /// <summary>
        /// Sets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="index">The block index to set the new block id for.</param>
        /// <param name="block">The new block id to set the block to.</param>
        /// <remarks>Wraps around if <paramref name="index"/> is out of range.</remarks>
        /// <param name="meta">The block meta data to set the block to.</param>
        void SetBlock(Index3 index, ushort block, int meta = 0);

        /// <summary>
        /// Sets the block id for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block id for.</param>
        /// <param name="y">The block index y-component to set the new block id for.</param>
        /// <param name="z">The block index z-component to set the new block id for.</param>
        /// <param name="block">The new block id to set the block to.</param>
        /// <param name="meta">The block meta data to set the block to.</param>
        /// <remarks>Wraps around if block index is out of range.</remarks>
        void SetBlock(int x, int y, int z, ushort block, int meta = 0);

        /// <summary>
        /// Sets the block info for a given flattened block index coordinate.
        /// </summary>
        /// <param name="flatIndex">The flattened index into <see cref="MetaData"/> and <see cref="Blocks"/> arrays.</param>
        /// <param name="blockInfo">The new block info to set the block to.</param>
        void SetBlock(int flatIndex, BlockInfo blockInfo);

        /// <summary>
        /// Gets the block meta data for a given block index coordinate local to the chunk
        /// </summary>
        /// <param name="x">The block index x-component to retrieve the block meta data for(in local block coordinates).</param>
        /// <param name="y">The block index y-component to retrieve the block meta data for(in local block coordinates).</param>
        /// <param name="z">The block index z-component to retrieve the block meta data for(in local block coordinates).</param>
        /// <returns>The block meta data at the given block coordinates; wraps around if out of range.</returns>
        int GetBlockMeta(int x, int y, int z);

        /// <summary>
        /// Sets the block meta data for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block meta data for.</param>
        /// <param name="y">The block index y-component to set the new block meta data for.</param>
        /// <param name="z">The block index z-component to set the new block meta data for.</param>
        /// <param name="meta">The new block meta data to set the block to.</param>
        /// <remarks>Wraps around if block index is out of range.</remarks>
        void SetBlockMeta(int x, int y, int z, int meta);

        /// <summary>
        /// Gets the block resources for a given block index coordinate local to the chunk
        /// </summary>
        /// <param name="x">The block index x-component to retrieve the block resources for(in local block coordinates).</param>
        /// <param name="y">The block index y-component to retrieve the block resources for(in local block coordinates).</param>
        /// <param name="z">The block index z-component to retrieve the block resources for(in local block coordinates).</param>
        /// <returns>The block resources at the given block coordinates; wraps around if out of range.</returns>
        ushort[] GetBlockResources(int x, int y, int z);

        /// <summary>
        /// Sets the block resources for a given block index coordinate.
        /// </summary>
        /// <param name="x">The block index x-component to set the new block resources for.</param>
        /// <param name="y">The block index y-component to set the new block resources for.</param>
        /// <param name="z">The block index z-component to set the new block resources for.</param>
        /// <param name="resources">The new block resources to set the block to.</param>
        /// <remarks>Wraps around if block index is out of range.</remarks>
        void SetBlockResources(int x, int y, int z, ushort[] resources);

        /// <summary>
        /// Set the chunk column this chunk is a part of.
        /// </summary>
        /// <param name="chunkColumn">THe parent chunk column to set the chunk as a child.</param>
        void SetColumn(IChunkColumn chunkColumn);

        /// <summary>
        /// Called when an update notification was received from a child.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void Update(Notifications.SerializableNotification notification);

        /// <summary>
        /// Called when an update notification was received from the parent.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void OnUpdate(SerializableNotification notification);

        /// <summary>
        /// Sets multiple blocks in this chunk.
        /// </summary>
        /// <param name="issueNotification">A value indicating whether the block changes should be notified.</param>
        /// <param name="blockInfos">The blocks to set.</param>
        void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos);

        /// <summary>
        /// Occurs when the chunk is changed.
        /// </summary>
        event Action<IChunk>? Changed;

        /// <summary>
        /// Flags the chunk as dirty. Meaning it needs to be updated for e.g. rendering.
        /// </summary>
        void FlagDirty();
    }
}
