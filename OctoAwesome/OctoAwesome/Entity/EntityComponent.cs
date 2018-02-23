using System;

namespace OctoAwesome.Entity
{
    /// <summary>
    /// Base Class for all Entity (Statespace) Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public Entity Entity { get; private set; }
        /// <summary>
        /// Constructor for <see cref="EntityComponent"/>.
        /// </summary>
        /// <param name="entity"></param>
        public EntityComponent(Entity entity)
        {
            if (Entity == null) throw new ArgumentNullException(nameof(entity));
            Entity = entity;
        }
    }
}
