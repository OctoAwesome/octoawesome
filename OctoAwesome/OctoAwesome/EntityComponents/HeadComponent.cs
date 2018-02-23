using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// HeadComponent
    /// </summary>
    public sealed class HeadComponent : EntityComponent
    {
        /// <summary>
        /// HeadPosition
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Tilt
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Angle
        /// </summary>
        public float Yaw { get; set; }

        public HeadComponent(Entity entity) : base(entity)
        {
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

            writer.Write(Offset.X);
            writer.Write(Offset.Y);
            writer.Write(Offset.Z);

            writer.Write(Tilt);
            writer.Write(Yaw);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);
            
            Offset = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            Tilt = reader.ReadSingle();
            Yaw = reader.ReadSingle();
        }
    }
}
