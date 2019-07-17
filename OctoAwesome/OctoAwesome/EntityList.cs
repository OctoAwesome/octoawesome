using OctoAwesome.EntityComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class EntityList : IEntityList
    {
        private List<Entity> entities;
        private IChunkColumn column;
        private readonly IResourceManager resourceManager;

        public EntityList(IChunkColumn column)
        {
            entities = new List<Entity>();
            this.column = column;
            resourceManager = TypeContainer.Get<IResourceManager>();
        }

        public int Count => entities.Count;

        public bool IsReadOnly => false;

        public void Add(Entity item)
        {
            entities.Add(item);
            column.ChangeCounter++;
        }

        public void Clear() => entities.Clear();

        public bool Contains(Entity item) => entities.Contains(item);

        public void CopyTo(Entity[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);

        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();

        public bool Remove(Entity item)
        {
            column.ChangeCounter++;
            return entities.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();

        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in entities)
            {
                if (entity.Components.ContainsComponent<PositionComponent>())
                {
                    var position = entity.Components.GetComponent<PositionComponent>();

                    if (position.Position.ChunkIndex.X != column.Index.X || position.Position.ChunkIndex.Y != column.Index.Y)
                    {
                        yield return new FailEntityChunkArgs()
                        {
                            Entity = entity,
                            CurrentChunk = new Index2(column.Index),
                            CurrentPlanet = column.Planet,
                            TargetChunk = new Index2(position.Position.ChunkIndex),
                            TargetPlanet = resourceManager.GetPlanet(position.Position.Planet),
                        };
                    }
                }
            }
        }
    }
}
