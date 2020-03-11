using OctoAwesome.Database;
using System;

namespace OctoAwesome
{
    public interface IDatabaseProvider
    {
        Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new();
        Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new();
        Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new();
    }
}