using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class BodyComponent : EntityComponent
    {
        public float Mass { get; set; }

        /// <summary>
        /// Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Die Körperhöhe des Spielers in Blocks
        /// </summary>
        public float Height { get; set; }

        public BodyComponent()
        {
            Mass = 1; //1kg
            Radius = 1;
            Height = 1;
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
        }
    }
}
