using Microsoft.Xna.Framework;
using OctoAwesome.Entities;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : PermanentEntity
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
        /// Bestimmt den Aktivierungsradius eines Spielers.
        /// </summary>
        public const int ACTIVATIONRANGE = 4;

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

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() : base(ACTIVATIONRANGE)
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
    }
}
