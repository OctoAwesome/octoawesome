using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome.Notifications
{

    public abstract class SerializableNotification : Notification, ISerializable
    {

        public abstract void Deserialize(BinaryReader reader);
        public abstract void Serialize(BinaryWriter writer);
    }
}
