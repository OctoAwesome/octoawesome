using OctoAwesome.Database;
using System;

namespace OctoAwesome
{
    public interface IDatabaseProvider
    {
        Database<T> GetDatabase<T>() where T : ITag, new();
        Database<T> GetDatabase<T>(Guid universeGuid) where T : ITag, new();
        Database<T> GetDatabase<T>(Guid universeGuid, int planetId) where T : ITag, new();
    }
}