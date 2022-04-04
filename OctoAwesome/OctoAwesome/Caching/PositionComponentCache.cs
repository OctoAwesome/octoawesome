using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome.Caching
{

    public class PositionComponentCache : Cache<Guid, PositionComponent>
    {
        private readonly IResourceManager resourceManager;

        private readonly Dictionary<Coordinate, CacheItem> positionComponentByCoor;

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
        protected override PositionComponent Load(Guid key)
            => resourceManager.GetComponent<PositionComponent>(key);
        protected PositionComponent GetBy(Coordinate position)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var cacheItem = positionComponentByCoor[position];
            cacheItem.LastAccessTime = DateTime.Now;
            return cacheItem.Value;
        }
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

        /// <summary>
        /// Tries to return the Component of the given Type or null
        /// </summary>
        /// <typeparam name="V">Component Type</typeparam>
        /// <returns>True if the component was found, false otherwise</returns>
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
