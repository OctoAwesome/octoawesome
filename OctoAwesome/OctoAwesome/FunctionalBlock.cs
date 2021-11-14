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
    public abstract class FunctionalBlock : ComponentContainer<IFunctionalBlockComponent>
    {
        public void Interact(GameTime gameTime, Entity entity)
        {
            OnInteract(gameTime, entity);
        }

        protected abstract void OnInteract(GameTime gameTime, Entity entity);
    }
}
