using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{

    public abstract class SerializableDatabaseContext<TTag, TObject> : DatabaseContext<TTag, TObject>
         where TTag : ITag, new()
         where TObject : ISerializable, new()
    {
        protected SerializableDatabaseContext(Database<TTag> database) : base(database)
        {
        }
        public override TObject Get(TTag key)
            => Serializer.Deserialize<TObject>(Database.GetValue(key).Content);

        protected void InternalRemove(TTag tag)
        {
            using (Database.Lock(Operation.Write))
                Database.Remove(tag);
        }
        protected void InternalAddOrUpdate(TTag tag, TObject value)
        {
            using (Database.Lock(Operation.Write))
                Database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }
    }
}
