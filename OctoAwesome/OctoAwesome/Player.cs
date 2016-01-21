using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : Entity
    {
        /// <summary>
        /// Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block"/> und <see cref="Entity"/> interagieren kann
        /// </summary>
        public const int SELECTIONRANGE = 8;

        /// <summary>
        /// Die Kraft, die der Spieler hat, um sich fortzubewegen
        /// </summary>
        public const float POWER = 600f;

        /// <summary>
        /// Die Kraft, die der Spieler hat, um in die Luft zu springen
        /// </summary>
        public const float JUMPPOWER = 400000f;

        /// <summary>
        /// Die Reibung die der Spieler mit der Umwelt hat
        /// </summary>
        public const float FRICTION = 60f;

        /// <summary>
        /// Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// TODO: Angle immer hübsch kürzen
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Die Körperhöhe des Spielers in Blocks
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler an Boden ist
        /// </summary>
        [XmlIgnore]
        public bool OnGround { get; set; }

        /// <summary>
        /// Gibt an. ob der Spieler grade sprintet
        /// </summary>
        public bool Sprint { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler grade kriecht
        /// </summary>
        public bool Crouch { get; set; }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Zurzeit nicht benutzt
        /// TODO: Ist das Nötig?
        /// </summary>
        public int InventorySlots { get; set; }

        /// <summary>
        /// Gibt an, ob der Flugmodus aktiviert ist.
        /// </summary>
        public bool FlyMode { get; set; }

        /// <summary>
        /// Das Inventar des Spielers.
        /// TODO: Persistieren...
        /// </summary>
        [XmlIgnore]
        public List<InventorySlot> Inventory { get; set; }

        public Player()
        {
            Position = new Coordinate(0, new Index3(0, 0, 100), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Inventory = new List<InventorySlot>();
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
            FlyMode = false;
        }

        /// <summary>
        /// TODO: Kommentieren
        /// </summary>
        /// <param name="data"></param>
        public override void SetData(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader br = new BinaryReader(stream);
                Position = new Coordinate(
                    br.ReadInt32(),
                    new Index3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32()),
                    new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                Radius = br.ReadSingle();
                Angle = br.ReadSingle();
                Height = br.ReadSingle();
            }
        }

        /// <summary>
        /// TODO: Kommentieren
        /// </summary>
        /// <returns></returns>
        public override byte[] GetData()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(Position.Planet);
                bw.Write(Position.GlobalBlockIndex.X);
                bw.Write(Position.GlobalBlockIndex.Y);
                bw.Write(Position.GlobalBlockIndex.Z);
                bw.Write(Position.BlockPosition.X);
                bw.Write(Position.BlockPosition.Y);
                bw.Write(Position.BlockPosition.Z);
                bw.Write(Radius);
                bw.Write(Angle);
                bw.Write(Height);

                byte[] buffer = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }
    }
}