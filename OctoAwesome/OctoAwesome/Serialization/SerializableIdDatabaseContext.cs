using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Serialization
{
    public sealed class SerializableIdDatabaseContext<TObject> : DatabaseContext<IdTag, int, TObject> where TObject : ISerializable, IIdentification, new()
    {
        public SerializableIdDatabaseContext(Database<IdTag> database) : base(database)
        {
        }

        public override void AddOrUpdate(TObject value) 
            => Database.AddOrUpdate(new IdTag(value.Id), new Value(Serializer.Serialize(value)));

        public override TObject Get(int key) 
            => Serializer.Deserialize<TObject>(Database.GetValue(new IdTag(key)).Content);

        public override void Remove(TObject value) 
            => Database.Remove(new IdTag(value.Id));
    }
}
