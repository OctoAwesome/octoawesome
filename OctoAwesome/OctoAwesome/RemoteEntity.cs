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
        public int RemoteID { get; set; }

        public RemoteEntity()
        {

        }

        public RemoteEntity(Entity originEntity)
        {
            var sendableComponents = Components.Where(c => c.Sendable);
            foreach (var component in sendableComponents)
                Components.AddComponent(component);
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            Components.Serialize(writer, definitionManager);
            base.Serialize(writer, definitionManager);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            Components.Deserialize(reader, definitionManager);
            base.Deserialize(reader, definitionManager);
        }
    }
}
