using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for all entities.
    /// </summary>
    public partial class Entity : ComponentContainer<IEntityComponent>
    {
        /// <summary>
        /// Initialized a new entity instance.
        /// </summary>
        public Entity() : base()
        {
                
        }

        /// <summary>
        /// Initializes a new entity instance with a predefined id and already existing components.
        /// </summary>
        /// <param name="id">The existing id of the entity.</param>
        /// <param name="components">The components that this entity should hold.</param>
        public Entity(Guid id, ComponentList<IComponent> components) : base(id, components)
        {
            
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.Assert(Simulation != null, nameof(Simulation) + " != null. Entity not part of a simulation.");
        }


    }
}
