﻿using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome
{
    public abstract class ComponentContainer : ISerializable, IIdentification, IContainsComponents, INotificationSubject<SerializableNotification>, IEquatable<ComponentContainer>
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// Reference to the active Simulation.
        /// </summary>
        public Simulation Simulation { get; internal set; }

        /// <summary>
        /// Contains only Components with notification interface implementation.
        /// </summary>
        protected readonly List<INotificationSubject<SerializableNotification>> notificationComponents;

        /// <summary>
        /// Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        public ComponentContainer()
        {
            notificationComponents = new();
            Id = Guid.Empty;
        }


        public virtual void RegisterDefault()
        {

        }



        public virtual void OnNotification(SerializableNotification notification)
        {
        }

        public void Interact(GameTime gameTime, Entity entity) => OnInteract(gameTime, entity);

        protected abstract void OnInteract(GameTime gameTime, Entity entity);
        public virtual void Push(SerializableNotification notification)
        {
            foreach (var component in notificationComponents)
                component?.OnNotification(notification);
        }

        public abstract void Serialize(BinaryWriter writer);
        public abstract void Deserialize(BinaryReader reader);
        public abstract bool ContainsComponent<T>();
        public abstract T GetComponent<T>();

        public override bool Equals(object obj) => Equals(obj as ComponentContainer);
        public bool Equals(ComponentContainer other) => other is not null && Id.Equals(other.Id);
        public override int GetHashCode() => HashCode.Combine(Id);

        public static bool operator ==(ComponentContainer left, ComponentContainer right) => EqualityComparer<ComponentContainer>.Default.Equals(left, right);
        public static bool operator !=(ComponentContainer left, ComponentContainer right) => !(left == right);
    }

    public abstract class ComponentContainer<TComponent> : ComponentContainer where TComponent : IComponent
    {
        /// <summary>
        /// Contains all Components.
        /// </summary>
        public ComponentList<TComponent> Components { get; private set; }

        /// <summary>
        /// Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        public ComponentContainer() : base()
        {
            Components = new(ValidateAddComponent, ValidateRemoveComponent, OnAddComponent, OnRemoveComponent);
        }

        protected void OnRemoveComponent(TComponent component)
        {

        }

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


        protected virtual void ValidateAddComponent(TComponent component)
        {
            //if (Simulation is not null)
            //    throw new NotSupportedException("Can't add components during simulation");
        }

        protected virtual void ValidateRemoveComponent(TComponent component)
        {
            //if (Simulation is not null)
            //    throw new NotSupportedException("Can't remove components during simulation");
        }

        public void Initialize(IResourceManager mananger) => OnInitialize(mananger);

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

        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Id.ToByteArray());

            Components.Serialize(writer);
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public override void Deserialize(BinaryReader reader)
        {
            Id = new Guid(reader.ReadBytes(16));
            Components.Deserialize(reader);
        }

        public override bool ContainsComponent<T>()
            => Components.ContainsComponent<T>();
        public override T GetComponent<T>()
            => Components.GetComponent<T>();
        public bool TryGetComponent<T>(out T component) where T : TComponent
            => Components.TryGetComponent<T>(out component);
    }
}
