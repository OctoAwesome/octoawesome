using Microsoft.Xna.Framework;
using OctoAwesome.Entities;
using System.IO;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity: IPosition, IRotatable
    {
        private float angle = 0f;

        /// <summary>
        /// Die Position der Entität
        /// </summary>
        public Coordinate Position { get; set; }

        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set { angle = MathHelper.WrapAngle(value); }
        }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        public float Tilt { get; set; }

        public virtual void Serialize(BinaryWriter writer)
        {
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);

            writer.Write(Angle);
            writer.Write(Tilt);
        }

        public virtual void Deserialize(BinaryReader reader)
        {
            // Position
            Position = new Coordinate(reader.ReadInt32(),
                new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));

            Angle = reader.ReadSingle();
            Tilt = reader.ReadSingle();
        }
    }
}
