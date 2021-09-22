using OctoAwesome.EntityComponents;
using System.Collections.Generic;
using System;
using System.Linq;

namespace OctoAwesome.Caching
{
    public class ChunkColumnCache : Cache<Index2, IChunkColumn>
    {
        private readonly IResourceManager resourceManager;
        private readonly IPlanet planet;

        public ChunkColumnCache(IResourceManager resourceManager, IPlanet planet)
        {
            this.resourceManager = resourceManager;
            this.planet = planet;
        }

        //TODO Implement Reference Count and return to pool
        sealed internal override void CollectGarbage()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                using var @lock = lockSemaphore.EnterExclusivScope();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                {
                    valueCache.Remove(element.Key, out _);
                }
            }
        }


        protected override IChunkColumn Load(Index2 key)
            => resourceManager.LoadChunkColumn(planet, key);

        private IChunkColumn GetBy(Index3 chunkColumnIndex, LoadingMode loadingMode)
        {
            if (planet.Id != chunkColumnIndex.Z)
                return default;
            return GetBy(new Index2(chunkColumnIndex), loadingMode);
        }


        public override TV Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            => key switch
            {
                Index2 chunkColumnIndex => GenericCaster<TV, IChunkColumn>.Cast(GetBy(chunkColumnIndex, loadingMode)),
                Index3 chunkColumnIndex => GenericCaster<TV, IChunkColumn>.Cast(GetBy(chunkColumnIndex, loadingMode)),
                _ => throw new NotSupportedException()
            };

    }
}
