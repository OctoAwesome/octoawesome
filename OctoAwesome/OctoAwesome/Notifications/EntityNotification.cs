using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class EntityNotification : SerializableNotification
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

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add)
                Entity = Serializer.Deserialize<RemoteEntity>(reader.ReadBytes(reader.ReadInt32()), definitionManager);
            else
                EntityId = reader.ReadInt32();

            var isNotification = reader.ReadBoolean();
            if (isNotification)
                Notification = Serializer.Deserialize<PropertyChangedNotification>(reader.ReadBytes(reader.ReadInt32()), definitionManager);
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null)
        {
            writer.Write((int)Type);

            if (Type == ActionType.Add)
            {
                var bytes = Serializer.Serialize(Entity, definitionManager);
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
                var bytes = Serializer.Serialize(Notification, definitionManager);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }

        public enum ActionType
        {
            None,
            Add,
            Remove,
            Update
        }
    }
}
