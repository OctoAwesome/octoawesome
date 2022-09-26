using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.IO;
using OctoAwesome.Extension;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Base Class for components that need to interact with a component container.
    /// </summary>
    /// <typeparam name="T">The component container that needs to be interacted with.</typeparam>
    public abstract class InstanceComponent<T> : Component, INotificationSubject<SerializableNotification>
        where T : ComponentContainer
    {
        private T? instance;

        /// <summary>
        /// Gets the reference to the <see cref="ComponentContainer{TComponent}"/>.
        /// </summary>
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
        public ulong InstanceTypeId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceComponent{T}"/> class.
        /// </summary>
        public InstanceComponent()
        {
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

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(InstanceId.ToByteArray());
            writer.Write(InstanceTypeId);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            InstanceId = new Guid(reader.ReadBytes(16));
            InstanceTypeId = reader.ReadUInt64();
        }

        /// <summary>
        /// Gets called when the instance was set to a new value.
        /// </summary>
        protected virtual void OnSetInstance()
        {

        }

        /// <inheritdoc />
        public virtual void OnNotification(SerializableNotification notification)
        {

        }

        /// <inheritdoc />
        public virtual void Push(SerializableNotification notification)
        {
            instance?.OnNotification(notification);
        }

    }
}
