using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public sealed class EntityNotification : SerializableNotification
    {
        public ActionType Type { get; set; }
        public int EntityId { get; private set; }
        public Entity Entity
        {
            get => entity; set
            {
                entity = value;
                EntityId = value.Id;
            }
        }

        public PropertyChangedNotification Notification { get; set; }

        private Entity entity;

        public EntityNotification()
        {

        }

        public EntityNotification(int id) : this()
        {
            EntityId = id;
        }

        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
                Entity = Serializer.Deserialize<RemoteEntity>(reader.ReadBytes(reader.ReadInt32()));
            else
                EntityId = reader.ReadInt32();

            var isNotification = reader.ReadBoolean();
            if (isNotification)
                Notification = Serializer.Deserialize<PropertyChangedNotification>(reader.ReadBytes(reader.ReadInt32()));
        }

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
                writer.Write(EntityId);
            }

            var subNotification = Notification != null;
            writer.Write(subNotification);
            if (subNotification)
            {                
                var bytes = Serializer.Serialize(Notification);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }

        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update,
            Request
        }
    }
}
