using OctoAwesome.EntityComponents;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OctoAwesome
{
    /// <summary>
    /// List of entities.
    /// </summary>
    public class EntityList : IEntityList
    {
        private readonly List<Entity> entities;
        private readonly IChunkColumn column;
        private readonly IResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityList"/> class.
        /// </summary>
        /// <param name="column">The column the entities in this list reside in.</param>
        public EntityList(IChunkColumn column)
        {
            entities = new List<Entity>();
            this.column = column;
            resourceManager = TypeContainer.Get<IResourceManager>();
        }

        /// <inheritdoc />
        public int Count => entities.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public void Add(Entity item)
        {
            entities.Add(item);
        }

        /// <inheritdoc />
        public void Clear() => entities.Clear();

        /// <inheritdoc />
        public bool Contains(Entity item) => entities.Contains(item);

        /// <inheritdoc />
        public void CopyTo(Entity[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();

        /// <inheritdoc />
        public bool Remove(Entity item)
        {
            return entities.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();

        /// <inheritdoc />
        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in entities)
            {
                if (entity.Components.Contains<PositionComponent>())
                {
                    var position = entity.Components.Get<PositionComponent>();

                    Debug.Assert(position != null, nameof(position) + " != null");
                    if (position.Position.ChunkIndex.X != column.Index.X || position.Position.ChunkIndex.Y != column.Index.Y)
                    {
                        yield return new FailEntityChunkArgs(
                            entity: entity,
                            currentChunk: column.Index,
                            currentPlanet: column.Planet,
                            targetChunk: new Index2(position.Position.ChunkIndex),
                            targetPlanet: resourceManager.GetPlanet(position.Position.Planet));
                    }
                }
            }
        }
    }
}
