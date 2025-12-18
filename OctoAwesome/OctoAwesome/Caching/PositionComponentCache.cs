using OctoAwesome.EntityComponents;
using OctoAwesome.Location;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Cache for <see cref="PositionComponent"/> class.
    /// </summary>
    public class PositionComponentCache : Cache<Guid, PositionComponent>
    {
        private readonly IResourceManager resourceManager;

        private readonly Dictionary<Index2, Dictionary<Guid, CacheItem>> positionComponentByCoor;

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
            LoadAllPositionComponentsFromResourceManager();
            base.Start();
        }

        private void LoadAllPositionComponentsFromResourceManager()
        {
            var positionComponents
                = resourceManager
                .GetAllComponents<PositionComponent>();

            foreach (var (id, component) in positionComponents)
            {
                _ = AddOrUpdateInternal(id, component);
            }

        }

        /// <inheritdoc/>
        protected override CacheItem AddOrUpdateInternal(Guid key, PositionComponent value)
        {
            var ci = base.AddOrUpdateInternal(key, value);
            using var @lock = lockSemaphore.EnterExclusiveScope();
            ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(positionComponentByCoor, value.Position.ChunkIndex.XY, out var exists);
            if (!exists)
            {
                list = new() { { key, ci } };
            }
            else
            {

                list[key] = ci;
            }

            return ci;
        }

        internal override bool Remove(Guid key, [MaybeNullWhen(false)] out PositionComponent positionComponent)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            if (base.Remove(key, out positionComponent))
            {
                foreach (var (_, dict) in positionComponentByCoor)
                {
                    if (dict.Remove(key))
                        return true; 
                }
            }

            return false;
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
            using (var @lock = lockSemaphore.EnterExclusiveScope())
            {
                foreach (var (index, dict) in positionComponentByCoor)
                {
                    if (index == position.ChunkIndex.XY)
                    {
                        foreach (var (_, ci) in dict)
                        {
                            if (ci.Value.Position == position)
                            {
                                ci.LastAccessTime = DateTime.UtcNow;
                                return ci.Value;
                            }
                        }
                    }
                }
            }
            return default!; //TODO Index exception of some sort
        }

        /// <summary>
        /// Gets a list of <see cref="PositionComponent"/> instances which are withing a specific chunk.
        /// </summary>
        /// <param name="chunkIndex">The chunk index to query the <see cref="PositionComponent"/> instances for.</param>
        /// <returns>A list of <see cref="PositionComponent"/> instances which are withing a specific chunk..</returns>
        protected List<PositionComponent> GetBy(Index2 chunkIndex)
        {
            using var @lock = lockSemaphore.EnterExclusiveScope();

            var list = new List<PositionComponent>();

            foreach (var (index, dict) in positionComponentByCoor)
            {
                if (index.X == chunkIndex.X
                    && index.Y == chunkIndex.Y)
                {
                    foreach (var (_, ci) in dict)
                    {
                        list.Add(ci.Value);
                        ci.LastAccessTime = DateTime.UtcNow;
                    }
                }
            }

            return list;
        }

        /// <inheritdoc />
        public override TValue? Get<TKey, TValue>(TKey key, LoadingMode loadingMode = LoadingMode.LoadIfNotExists)
            where TValue : default
            => key switch
            {
                Guid guid => GenericCaster<PositionComponent, TValue>.Cast(GetBy(guid, loadingMode)),
                Coordinate coordinate => GenericCaster<PositionComponent, TValue>.Cast(GetBy(coordinate)),
                Index2 chunkColumnIndex => GenericCaster<List<PositionComponent>, TValue>.Cast(GetBy(chunkColumnIndex)),
                //(IPlanet, Index2) index => GenericCaster<List<PositionComponent, TV>>.Cast(GetBy(index)),
                _ => throw new NotSupportedException()
            };
    }
}
