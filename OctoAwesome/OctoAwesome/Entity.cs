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

        public Entity() : base()
        {
                
        }
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
