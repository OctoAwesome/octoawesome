using System;
using System.Collections;
using System.Collections.Generic;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// List of Entities.
    /// </summary>
    public class EntityList : IEntityList
    {
        private List<Entity> entities;
        private IChunkColumn column;
        /// <summary>
        /// Constructor for <see cref="EntityList"/>
        /// </summary>
        /// <param name="column">ChucnkColumn</param>
        public EntityList(IChunkColumn column)
        {
            entities = new List<Entity>();
            this.column = column;
        }
        /// <summary>
        /// Count of Entities.
        /// </summary>
        public int Count => entities.Count;
        /// <summary>
        /// Indicates that this List is readolny or not.
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Adds an <see cref="Entity"/> to the List.
        /// </summary>
        /// <param name="item"></param>
        public void Add(Entity item)
        {
            entities.Add(item);
            column.ChangeCounter++;
        }
        /// <summary>
        /// Clear the List.
        /// </summary>
        public void Clear() => entities.Clear();
        /// <summary>
        /// Checks if an <see cref="Entity"/> is included.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(Entity item) => entities.Contains(item);
        /// <summary>
        /// Copy the internal <see cref="Array"/>.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(Entity[] array, int arrayIndex) => entities.CopyTo(array, arrayIndex);
        /// <summary>
        /// Remove an <see cref="Entity"/> from the List.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(Entity item)
        {
            column.ChangeCounter++;
            return entities.Remove(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Entity> GetEnumerator() => entities.GetEnumerator();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => entities.GetEnumerator();
        /// <summary>
        /// Checks if an <see cref="Entity"/> leaves a <see cref="IChunkColumn"/>?
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            foreach (var entity in entities)
            {
                //if (entity.Components.ContainsComponent<PositionComponent>())
                //{
                //    var position = entity.Components.GetComponent<PositionComponent>();
                //    ---> cut and passted from here
                //}
                if (entity.Position.ChunkIndex.X != column.Index.X || entity.Position.ChunkIndex.Y != column.Index.Y)
                {
                    yield return new FailEntityChunkArgs()
                    {
                        Entity = entity,
                        CurrentChunk = new Index2(column.Index),
                        CurrentPlanet = column.Planet,
                        TargetChunk = new Index2(entity.Position.ChunkIndex),
                        TargetPlanet = entity.Position.Planet,
                    };
                }
            }
        }
    }
}
