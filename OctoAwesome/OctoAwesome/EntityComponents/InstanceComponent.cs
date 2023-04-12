
using OctoAwesome.Serialization;
using System;
using System.IO;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using System.Diagnostics;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Base Class for components that need to interact with a component container.
    /// </summary>
    /// <typeparam name="T">The component container that needs to be interacted with.</typeparam>
    [Nooson]
    public abstract partial class InstanceComponent<T> : Component
        where T : ComponentContainer
    {
        private T? instance;
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IUpdateHub updateHub;
        private readonly IPool<PropertyChangedNotification> propertyChangedNotificationPool;

        /// <summary>
        /// Gets the reference to the <see cref="ComponentContainer{TComponent}"/>.
        /// </summary>
        [NoosonIgnore]
        public T Instance
        {
            get => NullabilityHelper.NotNullAssert(instance, $"{nameof(Instance)} was not initialized!");
            private set => instance = NullabilityHelper.NotNullAssert(value, $"{nameof(Instance)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets the unique identifier for the <see cref="Instance"/>.
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// Gets the instance type id.
        /// </summary>
        /// <seealso cref="SerializationIdTypeProvider"/>
        public ulong InstanceTypeId { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceComponent{T}"/> class.
        /// </summary>
        public InstanceComponent()
        {
            var typeContainer = TypeContainer.Get<ITypeContainer>();
            entityNotificationPool = typeContainer.Get<IPool<EntityNotification>>();
            updateHub = typeContainer.Get<IUpdateHub>();

            propertyChangedNotificationPool = typeContainer.Get<IPool<PropertyChangedNotification>>();
        }

        /// <summary>
        /// Sets the component container instance (<see cref="Instance"/>).
        /// </summary>
        /// <param name="value">The component container instance to set to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown if the <see cref="Instance"/> was already set.</exception>
        public void SetInstance(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (instance?.Id == value.Id)
                return;

            var type = value.GetType();
            if (instance != null)
            {
                throw new NotSupportedException("Can not change the " + type.Name);
            }

            InstanceTypeId = type.SerializationId();
            InstanceId = value.Id;
            Instance = value;
            OnSetInstance();
        }


        /// <summary>
        /// Gets called when the instance was set to a new value.
        /// </summary>
        protected virtual void OnSetInstance()
        {
            updateHub.ListenOn(DefaultChannels.Simulation).Subscribe(OnSimulationMessage);
        }


        protected override void OnPropertyChanged<T>(T value, string propertyName)
        {
            if (instance is Entity entity)
            {
                var updateNotification = propertyChangedNotificationPool.Rent();

                updateNotification.Issuer = GetType().Name;
                updateNotification.Property = propertyName;

                updateNotification.Value = Serializer.Serialize(this);

                var entityNotification = entityNotificationPool.Rent();
                entityNotification.Entity = entity;
                entityNotification.Type = EntityNotification.ActionType.Update;
                entityNotification.Notification = updateNotification;

                updateHub.PushNetwork(entityNotification, DefaultChannels.Simulation);
                entityNotification.Release();
            }
        }
        private void OnSimulationMessage(object obj)
        {
            if (obj is not EntityNotification entityNotification)
                return;
            if (InstanceId == Guid.Empty)
                return;
            //TODO Why InstanceID empty, should we use instance.Id or not?
            if (entityNotification.EntityId != InstanceId)
                return;

            switch (entityNotification.Type)
            {
                case EntityNotification.ActionType.Update:
                    EntityUpdate(entityNotification);
                    break;
            }
        }
        private void EntityUpdate(EntityNotification notification)
        {
            if (notification.Notification is PropertyChangedNotification changedNotification)
            {
                if (changedNotification.Issuer == GetType().Name)
                {
                    _ = Serializer.Deserialize(this, changedNotification.Value);
                }
            }
        }
    }
}
