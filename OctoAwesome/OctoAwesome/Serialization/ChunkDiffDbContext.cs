using OctoAwesome.Database;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDbContext : DatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        private readonly IPool<BlockChangedNotification> notificationBlockPool;

        public ChunkDiffDbContext(Database<ChunkDiffTag> database, IPool<BlockChangedNotification> blockPool)
            : base(database) => notificationBlockPool = blockPool;

        public override void AddOrUpdate(BlockChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
                InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)), value.BlockInfo);
        }

        public void AddOrUpdate(BlocksChangedNotification value)
        {
            using (Database.Lock(Operation.Write))
                value.BlockInfos.ForEach(b => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position)), b));
        }

        public IReadOnlyList<ChunkDiffTag> GetAllKeys()
            => Database.Keys;

        public override void Remove(BlockChangedNotification value)
            => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(value.BlockInfo.Position)));

        public void Remove(BlocksChangedNotification value)
            => value.BlockInfos.ForEach(b => InternalRemove(new ChunkDiffTag(value.ChunkPos, Chunk.GetFlatIndex(b.Position))));

        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (ChunkDiffTag tag in tags)
                InternalRemove(tag);
        }
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

        public override BlockChangedNotification Get(ChunkDiffTag key)
        {
            BlockChangedNotification notification = notificationBlockPool.Get();
            notification.BlockInfo = InternalGet(key);
            notification.ChunkPos = key.ChunkPositon;
            return notification;
        }
    }
}
