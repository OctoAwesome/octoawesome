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
    [Nooson]
    public partial class Entity : ComponentContainer<IEntityComponent>, ISerializable<Entity>
    {
        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.Assert(Simulation != null, nameof(Simulation) + " != null. Entity not part of a simulation.");
        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
