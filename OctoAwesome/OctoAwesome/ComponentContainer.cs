using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for classes containing components.
    /// </summary>
    public abstract class ComponentContainer : ISerializable, IIdentification, IComponentContainer, INotificationSubject<SerializableNotification>
    {
        /// <summary>
        /// Gets the Id of the container.
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Gets the reference to the active simulation; or <c>null</c> when no simulation is active.
        /// </summary>
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

        public void Interact(GameTime gameTime, Entity entity) => OnInteract(gameTime, entity);

        protected abstract void OnInteract(GameTime gameTime, Entity entity);
        /// <inheritdoc />
        public virtual void Push(SerializableNotification notification)
        {
            foreach (var component in notificationComponents)
                component?.OnNotification(notification);
        }

        /// <inheritdoc />
        public abstract void Serialize(BinaryWriter writer);

        /// <inheritdoc />
        public abstract void Deserialize(BinaryReader reader);

        /// <inheritdoc />
        public abstract bool ContainsComponent<T>();
        public abstract T GetComponent<T>();
    }

        /// <inheritdoc />
        public abstract T? GetComponent<T>();
    }
    /// <summary>
    /// Base class for classes containing components of a specific type.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components to contain.</typeparam>
    public abstract class ComponentContainer<TComponent> : ComponentContainer where TComponent : IComponent
    {
        /// <summary>
        /// Gets a list of all components this container holds.
        /// </summary>
        public ComponentList<TComponent> Components { get; }

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
            //if (Simulation is not null)
            //    throw new NotSupportedException("Can't add components during simulation");
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
            //if (Simulation is not null)
            //    throw new NotSupportedException("Can't remove components during simulation");
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
                    if (localChunkCache.LocalChunkCache != null)
                        return;

                    var positionComponent = Components.GetComponent<PositionComponent>();

                    if (positionComponent == null)
                        return;

                    localChunkCache.LocalChunkCache = new LocalChunkCache(positionComponent.Planet.GlobalChunkCache, 4, 2);
                }

            }
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());

            Components.Serialize(writer);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Id = new Guid(reader.ReadBytes(16));
            Components.Deserialize(reader);
        }

        /// <inheritdoc />
        public override bool ContainsComponent<T>()
            => Components.ContainsComponent<T>();

        /// <inheritdoc />
        public override T? GetComponent<T>() where T : default
            => Components.GetComponent<T>();
        public bool TryGetComponent<T>(out T component) where T : TComponent
            => Components.TryGetComponent<T>(out component);
    }
}
