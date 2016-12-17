using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OctoAwesome.Basics.EntityComponents
{
    public abstract class PowerComponent : EntityComponent
    {
        public float Power { get; set; }

        public Vector3 Direction { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

            writer.Write(Power);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            Power = reader.ReadSingle();
        }
    }
}
