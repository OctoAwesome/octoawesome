using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using OpenTK.Audio.OpenAL;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for functional blocks.
    /// </summary>
    public abstract class FunctionalBlock : ComponentContainer<IFunctionalBlockComponent>
    {
        /// <summary>
        /// Interact with this block.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="entity">The entity that interacts with this block.</param>
        public void Interact(GameTime gameTime, Entity entity)
        {
            OnInteract(gameTime, entity);
        }

        /// <summary>
        /// Called when an interaction on this functional block occurs.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="entity">The entity that interacted with this block.</param>
        protected abstract void OnInteract(GameTime gameTime, Entity entity);
    }
}
