using engenious;

using NonSucking.Framework.Serialization;

using OctoAwesome.Chunking;
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
    public abstract partial class ComponentContainer : IIdentification, IComponentContainer, ISerializable
    {
        /// <summary>
        /// Gets the Id of the container.
        /// </summary>
        public Guid Id { get; internal protected set; }

        /// <summary>
        /// Gets the reference to the active simulation; or <c>null</c> when no simulation is active.
        /// </summary>
        [NoosonIgnore]
        public Simulation? Simulation
        {
            get => simulation;
            internal set
            {
                if (value is null)
                    return;
                simulation = value;
                foreach (var item in Components)
                {
                    if (item is Component c)
                        c.OnParentSetting(this);
                }
            }
        }

        /// <summary>
        /// Gets a list of all components this container holds.
        /// </summary>
        public ComponentList<IComponent> Components { get; protected set; }

        private Simulation? simulation;
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer"/> class.
        /// </summary>
        protected ComponentContainer()
        {
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

        ///// <inheritdoc />
        //public abstract void Serialize(BinaryWriter writer);

        ///// <inheritdoc />
        //public abstract void Deserialize(BinaryReader reader);

        /// <inheritdoc />
        public bool ContainsComponent<T>()
            => Components.Contains<T>();

        /// <inheritdoc />
        public T? GetComponent<T>()
            => Components.Get<T>();

        /// <inheritdoc />
        public T? GetComponent<T>(int id)
            => Components.Get<T>(id);

        /// <summary>
        /// Tries to get the component of the component container
        /// </summary>
        /// <typeparam name="T">The Type of the component to search for</typeparam>
        /// <param name="component">The component to be returned</param>
        /// <returns>True if the component was found, otherwise false</returns>
        public bool TryGetComponent<T>([MaybeNullWhen(false)] out T component) where T : IComponent
            => Components.TryGet<T>(out component);


    }

    partial class ComponentContainer
    {
        ///<summary>
        ///Serializes the given <see cref="OctoAwesome.ComponentContainer"/> instance.
        ///</summary>
        ///<param name = "that">The instance to serialize.</param>
        ///<param name = "writer">The <see cref="System.IO.BinaryWriter"/> to serialize to.</param>
        public static void Serialize(OctoAwesome.ComponentContainer<IComponent> that, System.IO.BinaryWriter writer)
        {
            that.Serialize(writer);
        }

        ///<summary>
        ///Serializes this instance.
        ///</summary>
        ///<param name = "writer">The <see cref="System.IO.BinaryWriter"/> to serialize to.</param>
        public virtual void Serialize(System.IO.BinaryWriter writer)
        {
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
            var buffer = Id.ToByteArray();
#else
            var buffer = (System.Span<byte>)stackalloc byte[16];
            _ = Id.TryWriteBytes(buffer);
#endif
            writer.Write(buffer);
            Components.Serialize(writer);
        }

        ///<summary>
        ///Deserializes the properties of a <see cref="OctoAwesome.ComponentContainer{TComponent}"/> type.
        ///</summary>
        ///<param name = "reader">The <see cref="System.IO.BinaryReader"/> to deserialize from.</param>
        ///<param name = "id">The deserialized instance of the property <see cref="OctoAwesome.ComponentContainer.Id"/>.</param>
        ///<param name = "components">The deserialized instance of the property <see cref="OctoAwesome.ComponentContainer.Components"/>.</param>
        public static void DeserializeOut(System.IO.BinaryReader reader, out Guid id, out ComponentList<OctoAwesome.Components.IComponent> components)
        {
            id = default(System.Guid)!;
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_0_OR_GREATER)
            var buffer_id = reader.ReadBytes(16);
#else
            var buffer_id = (System.Span<byte>)stackalloc byte[16];
            reader.ReadBytes(buffer_id);
#endif
            id = new System.Guid(buffer_id);
            components = ComponentList<IComponent>.DeserializeStatic(reader);
        }

        ///<summary>
        ///Deserializes into <see cref="OctoAwesome.ComponentContainer{TComponent}"/> instance.
        ///</summary>
        ///<param name = "that">The instance to deserialize into.</param>
        ///<param name = "reader">The <see cref="System.IO.BinaryReader"/> to deserialize from.</param>
        public static void Deserialize(OctoAwesome.ComponentContainer that, System.IO.BinaryReader reader)
        {
            DeserializeOut(reader, out var id, out var components);
            that.Id = id;
            that.Components = components;
            components.Parent = that;
        }

        ///<summary>
        ///Deserializes into <see cref="OctoAwesome.ComponentContainer{TComponent}"/> instance.
        ///</summary>
        ///<param name = "reader">The <see cref="System.IO.BinaryReader"/> to deserialize from.</param>
        public virtual void Deserialize(System.IO.BinaryReader reader)
        {
            Deserialize(this, reader);
        }
    }

    /// <summary>
    /// Base class for classes containing components of a specific type.
    /// </summary>
    /// <typeparam name="TComponent">The type of the components to contain.</typeparam>
    public abstract partial class ComponentContainer<TComponent> :
        ComponentContainer, IUpdateable
        where TComponent : IComponent, ISerializable
    {
        /// <summary>
        /// The resource manager for loading resource assets.
        /// </summary>
        public IResourceManager? ResourceManager { get; internal set; }

        private List<IUpdateable> updateables = new();
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainer{TComponent}"/> class.
        /// </summary>
        protected ComponentContainer()
        {
            Components = new(ValidateAddComponent, ValidateRemoveComponent, OnAddComponent, OnRemoveComponent, this);
        }

        protected ComponentContainer(Guid id, ComponentList<IComponent> components)
        {
            Id = id;
            Components = components;
            Components.Parent = this;
        }

        /// <summary>
        /// Gets called when a component was removed from this container.
        /// </summary>
        /// <param name="component">The component that was removed.</param>
        protected void OnRemoveComponent(IComponent component)
        {

        }

        /// <summary>
        /// Gets called when a component was added to this container.
        /// </summary>
        /// <param name="component">The component that was added.</param>
        protected virtual void OnAddComponent(IComponent component)
        {
            component.Parent = this;

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
        protected virtual void ValidateAddComponent(IComponent component)
        {
        }

        /// <summary>
        /// Validates whether a component can be removed.
        /// </summary>
        /// <param name="component">The component to validate the remove for.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown when the component can not be removed in the current state. E.g. during simulation.
        /// </exception>
        protected virtual void ValidateRemoveComponent(IComponent component)
        {
        }

        /// <summary>
        /// Initializes the component container.
        /// </summary>
        public void Initialize()
        {
            OnInitialize();
        }

        /// <summary>
        /// Gets called when the component container is initializes.
        /// </summary>
        protected virtual void OnInitialize()
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
        public virtual void Update(GameTime gameTime)
        {
            foreach (var item in updateables)
                item.Update(gameTime);

        }
    }
}
