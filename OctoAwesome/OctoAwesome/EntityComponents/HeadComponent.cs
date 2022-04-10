using engenious;
using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component describing the head properties of an entity.
    /// </summary>
    public sealed class HeadComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the offset the head is located at relative to the entity position.
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Gets or sets the tilt of the head(in radians).
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Gets or sets the angle of the head(in radians).
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadComponent"/> class.
        /// </summary>
        public HeadComponent()
        {

        }


        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Offset.X);
            writer.Write(Offset.Y);
            writer.Write(Offset.Z);

            writer.Write(Tilt);
            writer.Write(Angle);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            Offset = new Vector3(x, y, z);


            Tilt = reader.ReadSingle();
            Angle = reader.ReadSingle();
        }
    }
}
