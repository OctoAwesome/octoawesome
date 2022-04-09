using System.Collections.Generic;
using System;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for a column of chunks.
    /// </summary>
    public interface IChunkColumn : ISerializable
    {
        /// <summary>
        /// Gets or sets a value indicating whether the column was already populated by <see cref="IMapPopulator"/>.
        /// </summary>
        bool Populated { get; set; }

        /// <summary>
        /// Gets the planet the chunk column is part of.
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Gets the index position of the chunk column.
        /// </summary>
        Index2 Index { get; }

        /// <summary>
        /// Gets the highest blocks inside the chunk column.
        /// </summary>
        int[,] Heights { get; }

        /// <summary>
        /// Gets the chunks in the column.
        /// </summary>
        IChunk[] Chunks { get; }

        /// <summary>
        /// Gets the block id for a given block index coordinate local to the chunk column
        /// </summary>
        /// <param name="index">The block index to retrieve the block id for(in local block coordinates).</param>
        /// <returns>The block id at the given block coordinates; wraps around if out of range.</returns>
        ushort GetBlock(Index3 index);

        /// <summary>
        /// Gets the block id for a given block index coordinate local to the chunk column
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
        /// Gets the block meta data for a given block index coordinate local to the chunk column
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
        /// Gets the block resources for a given block index coordinate local to the chunk column
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
        /// Called when an update notification was received from the parent.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void OnUpdate(Notifications.SerializableNotification notification);

        /// <summary>
        /// Called when an update notification was received from a child.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void Update(Notifications.SerializableNotification notification);

        /// <summary>
        /// Executes an action for each entity in the chunk column.
        /// </summary>
        /// <param name="action">The action to execute for each entity.</param>
        void ForEachEntity(Action<Entity> action);

        /// <summary>
        /// Gets an enumeration of <see cref="FailEntityChunkArgs"/>
        /// depicting entities that are not part of this chunk column anymore.
        /// </summary>
        /// <returns>The entities that are not part of this chunk column anymore.</returns>
        IEnumerable<FailEntityChunkArgs> FailChunkEntity();

        /// <summary>
        /// Remove an entity from this chunk column.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(Entity entity);

        /// <summary>
        /// Add an entity to this chunk column.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(Entity entity);

        /// <summary>
        /// Sets multiple blocks in this chunk column.
        /// </summary>
        /// <param name="issueNotification">A value indicating whether the block changes should be notified.</param>
        /// <param name="blockInfos">The blocks to set.</param>
        void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos);

        /// <summary>
        /// Flags the chunk column as dirty. Meaning it needs to be updated for e.g. rendering.
        /// </summary>
        /// <seealso cref="IChunk.FlagDirty"/>
        void FlagDirty();
    }
}
