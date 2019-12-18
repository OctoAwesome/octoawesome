using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public sealed class EntityDbContext : SerializableDatabaseContext<IdTag, Entity>
    {
        public EntityDbContext(Database<IdTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(Entity value)
            => InternalAddOrUpdate(new IdTag(value.Id), value);

        public IEnumerable<IdTag> GetAllKeys() => Database.Keys;

        public override void Remove(Entity value)
            => InternalRemove(new IdTag(value.Id));
    }
}
