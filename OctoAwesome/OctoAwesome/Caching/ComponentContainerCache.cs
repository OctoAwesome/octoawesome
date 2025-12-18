using OctoAwesome.Components;
using OctoAwesome.Serialization;

using System;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Cache for components contained in a <see cref="ComponentContainer{TContainer}"/>.
    /// </summary>
    /// <typeparam name="TContainer">The type of the component container which is cached.</typeparam>
    /// <typeparam name="TComponent">The type of the component contained in the container.</typeparam>
    public class ComponentContainerCache<TContainer, TComponent> : Cache<Guid, TContainer>
        where TContainer : ComponentContainer<TComponent>
        where TComponent : IComponent
    {
        private readonly IResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerCache{TContainer,TComponent}"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resource assets.</param>
        public ComponentContainerCache(IResourceManager resourceManager)
        {
            this.resourceManager = resourceManager;
        }

        /// <inheritdoc />
        protected override TContainer? Load(Guid key)
        {
            var componentContainer = resourceManager.LoadComponentContainer<TContainer, TComponent>(key);
            return componentContainer;
        }
    }
}
