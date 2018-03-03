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
        /// Eventhandler for released Componets.
        /// </summary>
        public event ReleaseEvent Released;
        /// <summary>
        /// Common Service.
        /// </summary>
        public IGameService Service { get; }
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
        public EntityComponent(Entity entity, IGameService service, bool needupdate) : base(needupdate)
        {
            Service = service;
            Entity = entity;
        }
        /// <summary>
        /// Constructor of <see cref="EntityComponent"/>
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="service">Service</param>
        public EntityComponent(Entity entity, IGameService service) : this(entity, service, true)
        {

        }
        /// <summary>
        /// Announce that this Component is released.
        /// </summary>
        protected virtual void Release()
        {
            Released?.Invoke(Entity, this);
        }
    }
}
