namespace OctoAwesome.Database
{
    public interface IDatabaseContext<in TTag, TObject> where TTag : ITag, new()
    {
        void AddOrUpdate(TObject value);
        TObject? Get(TTag key);
        void Remove(TObject value);
    }
}