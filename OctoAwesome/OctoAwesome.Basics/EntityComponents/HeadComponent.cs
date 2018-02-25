using engenious;
using System.IO;
using OctoAwesome.Entities;

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
        public float Angle { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

            writer.Write(Offset.X);
            writer.Write(Offset.Y);
            writer.Write(Offset.Z);

            writer.Write(Tilt);
            writer.Write(Angle);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            Offset = new Vector3(x, y, z);


            Tilt = reader.ReadSingle();
            Angle = reader.ReadSingle();
        }
    }
}
