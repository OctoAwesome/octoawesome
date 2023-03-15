using engenious;

using NonSucking.Framework.Serialization;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for classes containing components.
    /// </summary>
    public abstract partial class ComponentContainer : IIdentification, IComponentContainer, INotificationSubject<SerializableNotification>
    {
        /// <summary>
        /// Gets the Id of the container.
        /// </summary>
        public Guid Id { get; internal protected set; }

        /// <summary>
        /// Gets the reference to the active simulation; or <c>null</c> when no simulation is active.
        /// </summary>
        [NoosonIgnore]
        public Simulation? Simulation { get; internal set; }

        /// <summary>
        /// List of components with notification interface implementation.
        /// </summary>
        protected readonly List<INotificationSubject<SerializableNotification>> notificationComponents;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        protected ComponentContainer()
        {
            notificationComponents = new();
            Id = Guid.Empty;
        }

        /// <summary>
        /// Method used to register the default components of this <see cref="ComponentContainer"/>.
        /// </summary>
        public virtual void RegisterDefault()
        {

        }

        /// <inheritdoc />
        public override int GetHashCode()
            => Id.GetHashCode();

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Entity entity)
                return entity.Id == Id;

            return ReferenceEquals(this, obj);
        }

        /// <inheritdoc />
        public virtual void OnNotification(SerializableNotification notification)
        {
        }

        /// <summary>
        /// Used to interact with this component container
        /// </summary>
        /// <param name="gameTime">The current game time when the event happened</param>
        /// <param name="entity">The <see cref="Entity"/> that interacted with us</param>
        public void Interact(GameTime gameTime, Entity entity) => OnInteract(gameTime, entity);

        /// <summary>
        /// Called when this component container got interacted with
        /// </summary>
        /// <param name="gameTime">The current game time when the event happened</param>
        /// <param name="entity">The <see cref="Entity"/> that interacted with us</param>
        protected abstract void OnInteract(GameTime gameTime, Entity entity);
        /// <inheritdoc />
        public virtual void Push(SerializableNotification notification)
        {
            foreach (var component in notificationComponents)
                component.OnNotification(notification);
        }

        ///// <inheritdoc />
        //public abstract void Serialize(BinaryWriter writer);

        ///// <inheritdoc />
        //public abstract void Deserialize(BinaryReader reader);

        /// <inheritdoc />
        public abstract bool ContainsComponent<T>();
        /// <inheritdoc />
        public abstract T? GetComponent<T>();

    }
    /// <summary>
    /// Base class for classes containing components of a specific type.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components to contain.</typeparam>
    [Nooson]
    public abstract partial class ComponentContainer<TComponent> : 
        ComponentContainer, IUpdateable, ISerializable
        where TComponent : IComponent, ISerializable
    {
        /// <summary>
        /// Gets a list of all components this container holds.
        /// </summary>
        public ComponentList<TComponent> Components { get; protected set; }


        private List<IUpdateable> updateables = new();
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer{TComponent}"/> class.
        /// </summary>
        protected ComponentContainer()
        {
            Components = new(ValidateAddComponent, ValidateRemoveComponent, OnAddComponent, OnRemoveComponent);
        }

        /// <summary>
        /// Gets called when a component was removed from this container.
        /// </summary>
        /// <param name="component">The component that was removed.</param>
        protected void OnRemoveComponent(TComponent component)
        {

        }

        /// <summary>
        /// Gets called when a component was added to this container.
        /// </summary>
        /// <param name="component">The component that was added.</param>
        protected virtual void OnAddComponent(TComponent component)
        {
            if (component is InstanceComponent<ComponentContainer> instanceComponent)
                instanceComponent.SetInstance(this);

            //HACK: Remove PositionComponent Dependency
            //if (component is LocalChunkCacheComponent cacheComponent)
            //{
            //    if (cacheComponent.LocalChunkCache != null)
            //        return;

            //    var positionComponent = Components.GetComponent<PositionComponent>();

            //    if (positionComponent == null)
            //        return;

            //cacheComponent.LocalChunkCache = new LocalChunkCache(positionComponent.Planet.GlobalChunkCache, 4, 2);
            //}

            if (component is INotificationSubject<SerializableNotification> nofiticationComponent)
                notificationComponents.Add(nofiticationComponent);
            if (component is IUpdateable updateable)
                updateables.Add(updateable);
        }

        /// <summary>
        /// Validates whether a component can be added.
        /// </summary>
        /// <param name="component">The component to validate the add for.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when the component can not be added in the current state. E.g. during simulation.
        /// </exception>
        protected virtual void ValidateAddComponent(TComponent component)
        {
        }

        /// <summary>
        /// Validates whether a component can be removed.
        /// </summary>
        /// <param name="component">The component to validate the remove for.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when the component can not be removed in the current state. E.g. during simulation.
        /// </exception>
        protected virtual void ValidateRemoveComponent(TComponent component)
        {
        }

        /// <summary>
        /// Initializes the component container.
        /// </summary>
        /// <param name="manager">The resource manager for loading resource assets.</param>
        public void Initialize(IResourceManager manager)
        {
            OnInitialize(manager);
        }

        /// <summary>
        /// Gets called when the component container is initializes.
        /// </summary>
        /// <param name="manager">The resource manager for loading resource assets.</param>
        protected virtual void OnInitialize(IResourceManager manager)
        {
            foreach (var component in Components)
            {
                if (component is LocalChunkCacheComponent localChunkCache)
                {
                    if (!localChunkCache.Enabled)
                        return;

                    var positionComponent = Components.Get<PositionComponent>();

                    if (positionComponent == null)
                        return;

                    localChunkCache.LocalChunkCache = new LocalChunkCache(positionComponent.Planet.GlobalChunkCache, 4, 2);
                }

            }
        }

     
        /// <inheritdoc />
        public override bool ContainsComponent<T>()
            => Components.Contains<T>();

        /// <inheritdoc />
        public override T? GetComponent<T>() where T : default
            => Components.Get<T>();

        /// <summary>
        /// Tries to get the component of the component container
        /// </summary>
        /// <typeparam name="T">The Type of the component to search for</typeparam>
        /// <param name="component">The component to be returned</param>
        /// <returns>True if the component was found, otherwise false</returns>
        public bool TryGetComponent<T>([MaybeNullWhen(false)] out T component) where T : TComponent
            => Components.TryGet<T>(out component);


        /// <inheritdoc />
        public virtual void Update(GameTime gameTime)
        {
            foreach (var item in updateables)
                item.Update(gameTime);
            
        }
    }
}
