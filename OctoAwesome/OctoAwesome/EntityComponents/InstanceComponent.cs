using OctoAwesome.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class InstanceComponent<T> : Component, INotificationSubject<SerializableNotification> 
        where T: ComponentContainer
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public T Instance { get; private set; }

        public Guid InstanceId { get; set; }

        public ulong InstanceTypeId { get; private set; }

        public InstanceComponent()
        {
        }

        public void SetInstance(T instance)
        {
            if (instance is not null && Instance?.Id == instance.Id)
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

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(InstanceId.ToByteArray());
            writer.Write(InstanceTypeId);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            InstanceId = new Guid(reader.ReadBytes(16));
            InstanceTypeId = reader.ReadUInt64();
        }

        protected virtual void OnSetInstance()
        {

        }
        public virtual void OnNotification(SerializableNotification notification)
        {

        }
        public virtual void Push(SerializableNotification notification)
        {
            Instance?.OnNotification(notification);
        }
        
    }
}
