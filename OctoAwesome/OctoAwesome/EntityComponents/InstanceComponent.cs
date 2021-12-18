using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Base Class for components that need to interact with a component container.
    /// </summary>
    /// <typeparam name="T">The component container that needs to be interacted with.</typeparam>
    public abstract class InstanceComponent<T> : Component, INotificationSubject<SerializableNotification>
        where T : ComponentContainer
    {
        /// <summary>
        /// Gets the reference to the <see cref="ComponentContainer{TComponent}"/>.
        /// </summary>
        public T Instance { get; private set; }

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
        /// <param name="instance">The component container instance to set to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown if the <see cref="Instance"/> was already set.</exception>
        public void SetInstance(T instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));

            if (Instance?.Id == instance.Id)
                return;

            var type = instance.GetType();
            if (Instance != null)
            {
                throw new NotSupportedException("Can not change the " + type.Name);
            }

            InstanceTypeId = type.SerializationId();
            InstanceId = instance.Id;
            Instance = instance;
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
            Instance?.OnNotification(notification);
        }

    }
}
