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
            using var @lock = lockSemaphore.Wait();

            var positionComponents
                = resourceManager
                .GetAllComponents<PositionComponent>();

            foreach (var (id, component) in positionComponents)
            {
                var cacheItem = AddOrUpdate(id, component);
                positionComponentByCoor.Add(component.Position, cacheItem);
            }
        }

        internal override bool Remove(Guid key, out PositionComponent positionComponent)
        {
            using var @lock = lockSemaphore.Wait();

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
            using var @lock = lockSemaphore.Wait();

            var cacheItem = positionComponentByCoor[position];
            cacheItem.LastAccessTime = DateTime.Now;
            return cacheItem.Value;
        }

        public override TV Get<TK, TV>(TK key)
            => key switch
            {
                Guid guid => GenericCaster<TV, PositionComponent>.Cast(GetBy(guid)),
                Coordinate coordinate => GenericCaster<TV, PositionComponent>.Cast(GetBy(coordinate)),
                _ => throw new NotSupportedException()
            };
    }
}
