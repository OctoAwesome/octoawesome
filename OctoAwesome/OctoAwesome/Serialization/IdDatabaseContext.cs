using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Serialization
{
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<IdTag<int>, TObject> 
        where TObject : ISerializable, IIdentification, new()
    {
        public IdDatabaseContext(Database<IdTag<int>> database) : base(database)
        {
        }

        public override void AddOrUpdate(TObject value) 
            => InternalAddOrUpdate(new IdTag<int>(value.Id), value);

        public TObject Get(int key) 
            => Get(new IdTag<int>(key));

        public override void Remove(TObject value) 
           => InternalRemove(new IdTag<int>(value.Id));
    }
}
