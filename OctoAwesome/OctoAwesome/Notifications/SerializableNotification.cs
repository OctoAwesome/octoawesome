using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Base class for notifications that are serializable.
    /// </summary>
    public abstract class SerializableNotification : Notification, ISerializable
    {
        /// <inheritdoc />
        public abstract void Deserialize(BinaryReader reader);

        /// <inheritdoc />
        public abstract void Serialize(BinaryWriter writer);
    }
}
