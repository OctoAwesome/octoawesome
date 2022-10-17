using System;
using System.Linq;
using OctoAwesome.Chunking;
using OctoAwesome.Location;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Cache for <see cref="IChunkColumn"/> instances.
    /// </summary>
    public class ChunkColumnCache : Cache<Index2, IChunkColumn>
    {
        private readonly IResourceManager resourceManager;
        private readonly IPlanet planet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkColumnCache"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resource assets.</param>
        /// <param name="planet">The planet to cache the chunk columns for.</param>
        public ChunkColumnCache(IResourceManager resourceManager, IPlanet planet)
        {
            this.resourceManager = resourceManager;
            this.planet = planet;
        }

        //TODO Implement Reference Count and return to pool
        internal sealed override void CollectGarbage()
        {
            for (int i = valueCache.Count - 1; i >= 0; i--)
            {
                using var @lock = lockSemaphore.EnterExclusiveScope();

                var element = valueCache.ElementAt(i);
                if (element.Value.LastAccessTime.Add(ClearTime) < DateTime.Now)
                {
                    valueCache.Remove(element.Key, out _);
                }
            }
        }


        /// <inheritdoc />
        protected override IChunkColumn? Load(Index2 key)
            => resourceManager.LoadChunkColumn(planet, key);

        private IChunkColumn? GetBy(Index3 chunkColumnIndex, LoadingMode loadingMode)
        {
            if (planet.Id != chunkColumnIndex.Z)
                return default;
            return GetBy(new Index2(chunkColumnIndex), loadingMode);
        }


        /// <inheritdoc />
        public override TV? Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            where TV : default
            => key switch
            {
                Index2 chunkColumnIndex => GenericCaster<IChunkColumn, TV>.Cast(GetBy(chunkColumnIndex, loadingMode)),
                Index3 chunkColumnIndex => GenericCaster<IChunkColumn, TV>.Cast(GetBy(chunkColumnIndex, loadingMode)),
                _ => throw new NotSupportedException()
            };

    }
}
