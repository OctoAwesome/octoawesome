using OctoAwesome.Database;
using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkDiffDbContext : SerializableDatabaseContext<ChunkDiffTag, BlockChangedNotification>
    {
        public ChunkDiffDbContext(Database<ChunkDiffTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(BlockChangedNotification value)
            => InternalAddOrUpdate(new ChunkDiffTag(value.ChunkPos, value.FlatIndex), value);

        public IEnumerable<ChunkDiffTag> GetAllKeys() => Database.Keys;

        public override void Remove(BlockChangedNotification value)
            => InternalRemove(new ChunkDiffTag(value.ChunkPos, value.FlatIndex));

        public void Remove(params ChunkDiffTag[] tags)
        {
            foreach (var tag in tags)
                InternalRemove(tag);
        }
    }
}
