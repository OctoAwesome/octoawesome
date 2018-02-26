using engenious;
using System;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Indicates if the <see cref="EntityComponent"/> need Updates
        /// </summary>
        public bool NeedUpdate { get; }
        /// <summary>
        /// <see cref="Entities.Entity"/> of this <see cref="EntityComponent"/>
        /// </summary>
        public Entity Entity { get; }
        /// <summary>
        /// Constructor of <see cref="EntityComponent"/>
        /// </summary>
        public EntityComponent(Entity entity, bool needupdate)
        {
            NeedUpdate = needupdate;
            Entity = entity;
        }
        /// <summary>
        /// Update method.
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);
        


    }
}
