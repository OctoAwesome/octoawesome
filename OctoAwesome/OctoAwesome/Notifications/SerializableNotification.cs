using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public abstract class SerializableNotification : Notification, ISerializable
    {

        public abstract void Deserialize(BinaryReader reader, IDefinitionManager definitionManager = null);

        public abstract void Serialize(BinaryWriter writer, IDefinitionManager definitionManager = null);
    }
}
