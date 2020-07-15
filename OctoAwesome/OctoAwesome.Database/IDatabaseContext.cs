namespace OctoAwesome.Database
{
    public interface IDatabaseContext<Tag, TObject> where Tag : ITag, new()
    {
        void AddOrUpdate(TObject value);
        TObject Get(Tag key);
        void Remove(TObject value);
    }
}