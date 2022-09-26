using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.IO;
using OctoAwesome.Extension;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notifications caused by entities.
    /// </summary>
    public sealed class EntityNotification : SerializableNotification
    {
        /// <summary>
        /// Gets or sets the action type that caused the notification.
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        /// Gets or sets the id of the entity that caused the notification.
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity that caused the notification.
        /// </summary>
        public Entity Entity
        {
            get => NullabilityHelper.NotNullAssert(entity, $"{nameof(Entity)} was not initialized!");
            set
            {
                entity = NullabilityHelper.NotNullAssert(value, $"{nameof(Entity)} cannot be initialized with null!");
                EntityId = value.Id;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating if the simulation should overwrite an existing entity with the same id on add
        /// </summary>
        public bool OverwriteExisting { get; set; }

        /// <summary>
        /// Gets or sets the underlying property changed notification.
        /// </summary>
        public PropertyChangedNotification? Notification { get; set; }

        private Entity? entity;
        private readonly IPool<PropertyChangedNotification> propertyChangedNotificationPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotification"/> class.
        /// </summary>
        public EntityNotification()
        {
            propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();
            OverwriteExisting = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotification"/> class.
        /// </summary>
        /// <param name="id">The id of the entity that caused the notification.</param>
        public EntityNotification(Guid id) : this()
        {
            EntityId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityNotification"/> class.
        /// </summary>
        public EntityNotification(ActionType type, Entity entity) : this(entity.Id)
        {
            Type = type;
            Entity = entity;
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
                Entity = Serializer.Deserialize<RemoteEntity>(reader.ReadBytes(reader.ReadInt32()));
            else
                EntityId = new Guid(reader.ReadBytes(16));

            var isNotification = reader.ReadBoolean();
            if (isNotification)
                Notification = Serializer.DeserializePoolElement(
                    propertyChangedNotificationPool, reader.ReadBytes(reader.ReadInt32()));
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((int)Type);

            if (Type == ActionType.Add)
            {
                var bytes = Serializer.Serialize(Entity);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
            else
            {
                writer.Write(EntityId.ToByteArray());
            }

            var subNotification = Notification != null;
            writer.Write(subNotification);
            if (subNotification)
            {
                var bytes = Serializer.Serialize(Notification!);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }

        /// <inheritdoc />
        protected override void OnRelease()
        {
            Notification?.Release();

            Type = default;
            entity = default;
            Notification = default;

            base.OnRelease();
        }

        /// <summary>
        /// Enumeration of entity notification action types.
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// Nothing happened to the entity.
            /// </summary>
            None,
            /// <summary>
            /// A new entity was added.
            /// </summary>
            Add,
            /// <summary>
            /// An entity was removed.
            /// </summary>
            Remove,
            /// <summary>
            /// An entity was updated.
            /// </summary>
            Update,
            /// <summary>
            /// An entity was requested.
            /// </summary>
            Request
        }
    }
}
