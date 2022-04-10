using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Cache for <see cref="PositionComponent"/> class.
    /// </summary>
    public class PositionComponentCache : Cache<Guid, PositionComponent>
    {
        private readonly IResourceManager resourceManager;

        private readonly Dictionary<Coordinate, CacheItem> positionComponentByCoor;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionComponentCache"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resource assets.</param>
        public PositionComponentCache(IResourceManager resourceManager)
        {
            positionComponentByCoor = new();

            this.resourceManager = resourceManager;
        }

        internal override void Start()
        {
            var positionComponents
                = resourceManager
                .GetAllComponents<PositionComponent>();

            foreach (var (id, component) in positionComponents)
            {
                var cacheItem = AddOrUpdate(id, component);
                using var @lock = lockSemaphore.EnterExclusiveScope();
                positionComponentByCoor.Add(component.Position, cacheItem);
            }

            base.Start();
        }

        internal override bool Remove(Guid key, [MaybeNullWhen(false)] out PositionComponent positionComponent)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var returnValue = base.Remove(key, out positionComponent);

            if (returnValue)
            {
                return returnValue
                     && positionComponentByCoor
                         .Remove(positionComponent.Position);
            }
            else
            {
                return returnValue;
            }
        }

        /// <inheritdoc />
        protected override PositionComponent Load(Guid key)
            => resourceManager.GetComponent<PositionComponent>(key);

        /// <summary>
        /// Gets the <see cref="PositionComponent"/> with an exact coordinate.
        /// </summary>
        /// <param name="position">The exact position to get the <see cref="PositionComponent"/> for.</param>
        /// <returns>The <see cref="PositionComponent"/> with the exact coordinate.</returns>
        protected PositionComponent GetBy(Coordinate position)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var cacheItem = positionComponentByCoor[position];
            cacheItem.LastAccessTime = DateTime.Now;
            return cacheItem.Value;
        }

        /// <summary>
        /// Gets a list of <see cref="PositionComponent"/> instances which are withing a specific chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index to query the <see cref="PositionComponent"/> instances for.</param>
        /// <returns>A list of <see cref="PositionComponent"/> instances which are withing a specific chunk..</returns>
        protected List<PositionComponent> GetBy(Index3 chunkIndex)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var list = new List<PositionComponent>();

            foreach (var component in positionComponentByCoor)
            {
                var key = component.Key;
                var normalizedChunkIndex = key.ChunkIndex;
                normalizedChunkIndex.NormalizeXY(component.Value.Value.Planet.Size);


                if (key.Planet == chunkIndex.Z
                    && normalizedChunkIndex.X == chunkIndex.X
                    && normalizedChunkIndex.Y == chunkIndex.Y)
                {
                    list.Add(component.Value.Value);
                    component.Value.LastAccessTime = DateTime.Now;
                }
            }

            return list;
        }

        /// <inheritdoc />
        public override TValue Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            => key switch
            {
                Guid guid => GenericCaster<PositionComponent, TValue>.Cast(GetBy(guid, loadingMode)),
                Coordinate coordinate => GenericCaster<PositionComponent, TValue>.Cast(GetBy(coordinate)),
                Index3 chunkColumnIndex => GenericCaster<List<PositionComponent>, TValue>.Cast(GetBy(chunkColumnIndex)),
                //(IPlanet, Index2) index => GenericCaster<List<PositionComponent, TV>>.Cast(GetBy(index)),
                _ => throw new NotSupportedException()
            };
    }
}
