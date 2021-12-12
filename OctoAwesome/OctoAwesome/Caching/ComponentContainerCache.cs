using OctoAwesome.Components;
using System;

namespace OctoAwesome.Caching
{

    public class ComponentContainerCache<TContainer, TComponent> : Cache<Guid, TContainer> 
        where TContainer : ComponentContainer<TComponent>
        where TComponent : IComponent
    {
        private readonly IResourceManager resourceManager;

        public ComponentContainerCache(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }
        protected override TContainer Load(Guid key) 
            => resourceManager.LoadComponentContainer<TContainer, TComponent>(key);
    }
}
