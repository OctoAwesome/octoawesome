using engenious;
using OctoAwesome.Common;
using System;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// EventHandler for released EntityComponents
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="component">EntityComponent</param>
    public delegate void ReleaseEvent(Entity entity, EntityComponent component);
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// <see cref="Entities.Entity"/> of this <see cref="EntityComponent"/>
        /// </summary>
        public Entity Entity { get; private set; }
        /// <summary>
        /// Constructor of <see cref="EntityComponent"/>
        /// </summary>
        /// <param name="needupdate">Indicates that the <see cref="EntityComponent"/> need updates</param>
        public EntityComponent(bool needupdate) : base(needupdate)
        {
        }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public EntityComponent() : base()
        {

        }
        /// <summary>
        /// Set the Entity for this Component
        /// </summary>
        /// <param name="entity"></param>
        public void SetEntity(Entity entity)
        {
            if (entity == null) return;
            if (Entity != null) return;
            OnSetEntity(entity);
            Entity = entity;
        }
        /// <summary>
        /// Called during SetEntity
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void OnSetEntity(Entity entity)
        {

        }
    }
}
