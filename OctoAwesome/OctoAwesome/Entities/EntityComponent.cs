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
        /// Common Service.
        /// </summary>
        public IGameService Service { get; }
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
        /// <param name="entity">Entity</param>
        /// <param name="service">Service</param>
        /// <param name="needupdate">Indicates if the <see cref="EntityComponent"/> need updates</param>
        public EntityComponent(Entity entity, IGameService service, bool needupdate)
        {
            Service = service;
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
