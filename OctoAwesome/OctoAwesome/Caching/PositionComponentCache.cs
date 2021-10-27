using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;

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
                using var @lock = lockSemaphore.EnterExclusivScope();
                positionComponentByCoor.Add(component.Position, cacheItem);
            }

            base.Start();
        }

        internal override bool Remove(Guid key, out PositionComponent positionComponent)
        {
            using var @lock = lockSemaphore.EnterExclusivScope();

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
            using var @lock = lockSemaphore.EnterExclusivScope();

            var cacheItem = positionComponentByCoor[position];
            cacheItem.LastAccessTime = DateTime.Now;
            return cacheItem.Value;
        }

        protected List<PositionComponent> GetBy(Index3 position)
        {
            using var @lock = lockSemaphore.EnterExclusivScope();

            var list = new List<PositionComponent>();

            foreach (var component in positionComponentByCoor)
            {
                var key = component.Key;
                var normalizedChunkIndex = key.ChunkIndex;
                normalizedChunkIndex.NormalizeXY(component.Value.Value.Planet.Size);


                if (key.Planet == position.Z
                    && normalizedChunkIndex.X == position.X
                    && normalizedChunkIndex.Y == position.Y)
                {
                    list.Add(component.Value.Value);
                    component.Value.LastAccessTime = DateTime.Now;
                }
            }

            return list;
        }

        internal override void CollectGarbage()
        {
            //Intended, no garbage collection neccessary and needed
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="key"></param>
        /// <param name="loadingMode">Is ignored, because this cache load everything on startup and therefore is always cached</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public override TV Get<TK, TV>(TK key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            => key switch
            {
                Guid guid => GenericCaster<TV, PositionComponent>.Cast(GetBy(guid, loadingMode)),
                Coordinate coordinate => GenericCaster<TV, PositionComponent>.Cast(GetBy(coordinate)),
                Index3 chunkColumnIndex => GenericCaster<TV, List<PositionComponent>>.Cast(GetBy(chunkColumnIndex)),
                //(IPlanet, Index2) index => GenericCaster<TV, List<PositionComponent>>.Cast(GetBy(index)),
                _ => throw new NotSupportedException()
            };
    }
}
