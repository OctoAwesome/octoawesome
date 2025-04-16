using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for all poolable entities.
    /// </summary>
    public partial class PoolableEntity : Entity, IPoolElement
    {
        private IPool? pool;

        /// <summary>
        /// Initialized a new entity instance.
        /// </summary>
        public PoolableEntity() : base()
        {
                
        }

        /// <summary>
        /// Initializes a new entity instance with a predefined id and already existing components.
        /// </summary>
        /// <param name="id">The existing id of the entity.</param>
        /// <param name="components">The components that this entity should hold.</param>
        public PoolableEntity(Guid id, ComponentList<IComponent> components) : base(id, components)
        {
            
        }

        public void Init(IPool pool)
        {
            this.pool = pool;
        }

        public void Release()
        {
            pool?.Return(this);
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.Assert(Simulation != null, nameof(Simulation) + " != null. Entity not part of a simulation.");
        }


    }
}
