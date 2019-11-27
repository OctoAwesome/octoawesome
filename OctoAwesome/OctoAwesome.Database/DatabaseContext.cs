using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    public abstract class DatabaseContext<Tag, TObject> where Tag : ITag, new()
    {
        protected Database<Tag> Database { get; }

        protected DatabaseContext(Database<Tag> database)
        {
            Database = database;
        }

        public abstract TObject Get(Tag key);

        public abstract void AddOrUpdate(TObject value);

        public abstract void Remove(TObject value);
    }
}
