using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OctoAwesome
{
    public sealed class Player : Entity
    {
        public const int SELECTIONRANGE = 8;

        public const float POWER = 600f;

        public const float JUMPPOWER = 400000f;

        public const float FRICTION = 60f;

        public float Radius { get; set; }

        // TODO: Angle immer hübsch kürzen
        public float Angle { get; set; }

        public float Height { get; set; }

        public bool OnGround { get; set; }

        public float Tilt { get; set; }

        public int InventorySlots { get; set; }

        public bool FlyMode { get; set; }

        [XmlIgnore]
        public List<InventorySlot> Inventory { get; set; }

        public Player()
        {
            // Position = new Coordinate(0, new Index3(8038, 73908, 80), Vector3.Zero);
            // Position = new Coordinate(0, new Index3(1000, 1000, 150), Vector3.Zero);
            Position = new Coordinate(0, new Index3(10, 10, 50), Vector3.Zero);
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
