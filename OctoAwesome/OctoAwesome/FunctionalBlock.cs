using engenious;
using OctoAwesome.Components;

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
