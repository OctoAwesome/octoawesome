using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
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
        /// Gibt die Anzahl Tools in der Toolbar an.
        /// </summary>
        public const int TOOLCOUNT = 10;

        /// <summary>
        /// Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }

        private float angle = 0f;

        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set { angle = MathHelper.WrapAngle(value); }
        }

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
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Gibt an, ob der Flugmodus aktiviert ist.
        /// </summary>
        public bool FlyMode { get; set; }

        /// <summary>
        /// Maximales Gewicht im Inventar.
        /// </summary>
        public float InventoryLimit { get; set; }

        /// <summary>
        /// Das Inventar des Spielers.
        /// TODO: Persistieren...
        /// </summary>
        [XmlIgnore]
        public List<InventorySlot> Inventory { get; set; }

        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        [XmlIgnore]
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player()
        {
            Position = new Coordinate(0, new Index3(0, 0, 100), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Inventory = new List<InventorySlot>();
            Tools = new InventorySlot[TOOLCOUNT];
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
            FlyMode = false;
            InventoryLimit = 1000;
        }

        public override void Serialize(BinaryWriter writer)
        {
            // Entity
            base.Serialize(writer);

            // Radius
            writer.Write(Radius);

            // Angle
            writer.Write(Angle);

            // Height
            writer.Write(Height);

            // Tilt
            writer.Write(Tilt);

            // FlyMode
            writer.Write(FlyMode);

            // Inventory Limit
            // TODO: Überlegen was damit passiert

            // Inventory ???


            // Inventory Tools (Index auf Inventory)
        }

        public override void Deserialize(BinaryReader reader)
        {
            // Entity
            base.Deserialize(reader);

            // Radius
            Radius = reader.ReadSingle();

            // Angle
            Angle = reader.ReadSingle();

            // Height
            Height = reader.ReadSingle();

            // Tilt
            Tilt = reader.ReadSingle();

            // FlyMode
            FlyMode = reader.ReadBoolean();

            // Inventory Limit

            // Inventory ???

            // Inventory Tools (Index auf Inventory)
        }
    }
}
