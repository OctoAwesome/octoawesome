using OctoAwesome.Chunking;
using OctoAwesome.Database;
using OctoAwesome.Information;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Database context for chunk differences using <see cref="BlockChangedNotification"/>.
    /// </summary>
    public sealed class ChunkDiffDbContext : DatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        private readonly IPool<BlockChangedNotification> notificationBlockPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkDiffDbContext"/> class.
        /// </summary>
        /// <param name="database">The underlying database for this context.</param>
        /// <param name="blockPool">
        /// The memory pool for pooling <see cref="BlockChangedNotification"/> instances.
        /// </param>
        public ChunkDiffDbContext(Database<ChunkDiffTag> database, IPool<BlockChangedNotification> blockPool)
            : base(database) => notificationBlockPool = blockPool;

        /// <inheritdoc />
        public override void AddOrUpdate(BlockChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
                InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)), value.BlockInfo);
        }

        /// <summary>
        /// Adds or updates multiple values from a <see cref="BlocksChangedNotification"/> to the database context.
        /// </summary>
        /// <param name="value">The collection of values to add or update.</param>
        public void AddOrUpdate(BlocksChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
                value.BlockInfos.ForEach(b => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position)), b));
        }

        /// <summary>
        /// Get all key tags in this database context.
        /// </summary>
        /// <returns>The <see cref="ChunkDiffTag"/> identifying keys.</returns>
        public IReadOnlyList<ChunkDiffTag> GetAllKeys()
            => Database.Keys;

        /// <inheritdoc />
        public override void Remove(BlockChangedNotification value)
            => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)));

        /// <summary>
        /// Removes multiple values from a <see cref="BlocksChangedNotification"/> from the database context.
        /// </summary>
        /// <param name="value">The collection of values to remove.</param>
        public void Remove(BlocksChangedNotification value)
            => value.BlockInfos.ForEach(b => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position))));

        /// <summary>
        /// Remove multiple values identified by the given tag collection from the database context.
        /// </summary>
        /// <param name="tags">The collection of identifying tags for the values to remove.</param>
        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (ChunkDiffTag tag in tags)
                InternalRemove(tag);
        }

        /// <summary>
        /// Remove multiple values identified by the given tag collection from the database context.
        /// </summary>
        /// <param name="tags">The collection of identifying tags for the values to remove.</param>
        public void Remove(IReadOnlyList<ChunkDiffTag> tags)
        {
            foreach (ChunkDiffTag tag in tags)
                InternalRemove(tag);
        }

        private void InternalRemove(ChunkDiffTag tag)
        {
            using (Database.Lock(Operation.Write))
                Database.Remove(tag);
        }

        private void InternalAddOrUpdate(ChunkDiffTag tag, BlockInfo blockInfo)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory))
            {
                BlockInfo.Serialize(writer, blockInfo);
                Database.AddOrUpdate(tag, new Value(memory.ToArray()));
            }
        }

        private BlockInfo InternalGet(ChunkDiffTag tag)
        {
            Value value = Database.GetValue(tag);
            using (var memory = new MemoryStream(value.Content))
            using (var reader = new BinaryReader(memory))
            {
                return BlockInfo.Deserialize(reader);
            }
        }

        /// <inheritdoc />
        public override BlockChangedNotification Get(ChunkDiffTag key)
        {
            BlockChangedNotification notification = notificationBlockPool.Rent();
            notification.BlockInfo = InternalGet(key);
            notification.ChunkPos = key.ChunkPositon;
            return notification;
        }
    }
}
