using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class RemoteEntity : Entity
    {
        public RemoteEntity()
        {

        }

        public RemoteEntity(Entity originEntity)
        {
            var sendableComponents = Components.Where(c => c.Sendable);
            foreach (var component in sendableComponents)
                Components.AddComponent(component);
            Id = originEntity.Id;
        }

        public override void Serialize(BinaryWriter writer)
        {
            Components.Serialize(writer);
            base.Serialize(writer);
        }

        public override void Deserialize(BinaryReader reader)
        {
            Components.Deserialize(reader);
            base.Deserialize(reader);
        }
    }
}
