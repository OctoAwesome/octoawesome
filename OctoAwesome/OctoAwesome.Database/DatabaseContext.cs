namespace OctoAwesome.Database
{

    public abstract class DatabaseContext<TTag, TObject> : IDatabaseContext<TTag, TObject> where TTag : ITag, new()
    {

        protected Database<TTag> Database { get; }

        protected DatabaseContext(Database<TTag> database)
        {
            Database = database;
        }
        public abstract TObject? Get(TTag key);
        public abstract void AddOrUpdate(TObject value);
        public abstract void Remove(TObject value);
    }
}
