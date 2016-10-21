using System.Collections.Generic;
using System.Xml.Serialization;
using engenious;
using System.IO;
using System.Linq;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class PlayerComponent : Component<PlayerComponent>
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
        //public float Radius { get; set; }

        //private float angle = 0f;

        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// </summary>
        //public float Angle
        //{
        //    get { return angle; }
        //    set { angle = MathHelper.WrapAngle(value); }
        //}

        /// <summary>
        /// Die Körperhöhe des Spielers in Blocks
        /// </summary>
        //public float Height { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler an Boden ist
        /// </summary>
        //public bool OnGround { get; set; }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        //public float Tilt { get; set; }

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
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public PlayerComponent()
        {
            //Position = new Coordinate(0, new Index3(0, 0, 100), Vector3.Zero);
            //Velocity = new Vector3(0, 0, 0);
            Inventory = new List<InventorySlot>();
            Tools = new InventorySlot[TOOLCOUNT];
            //Radius = 0.75f;
            //Angle = 0f;
            //Height = 3.5f;
            //Mass = 100;
            FlyMode = false;
            InventoryLimit = 1000;
        }

        /// <summary>
        /// Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        //public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        //{
        //    // Entity
        //    base.Serialize(writer, definitionManager);

        //    // Radius
        //    writer.Write(Radius);

        //    // Angle
        //    writer.Write(Angle);

        //    // Height
        //    writer.Write(Height);

        //    // Tilt
        //    writer.Write(Tilt);

        //    // FlyMode
        //    writer.Write(FlyMode);

        //    // Inventory Limit
        //    // TODO: Überlegen was damit passiert

        //    // Inventory
        //    writer.Write(Inventory.Count);
        //    foreach (var slot in Inventory)
        //    {
        //        writer.Write(slot.Definition.GetType().FullName);
        //        writer.Write(slot.Amount);
        //    }

        //    // Inventory Tools (Index auf Inventory)
        //    byte toolCount = (byte)Tools.Count(t => t != null);
        //    writer.Write(toolCount);
        //    for (byte i = 0; i < Tools.Length; i++)
        //    {
        //        if (Tools[i] == null)
        //            continue;

        //        writer.Write(i);
        //        writer.Write(Tools[i].Definition.GetType().FullName);
        //    }
        //}

        ///// <summary>
        ///// Deserialisiert den Player aus dem angegebenen BinaryReader.
        ///// </summary>
        ///// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        ///// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        //public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        //{
        //    // Entity
        //    base.Deserialize(reader, definitionManager);

        //    // Radius
        //    Radius = reader.ReadSingle();

        //    // Angle
        //    Angle = reader.ReadSingle();

        //    // Height
        //    Height = reader.ReadSingle();

        //    // Tilt
        //    Tilt = reader.ReadSingle();

        //    // FlyMode
        //    FlyMode = reader.ReadBoolean();

        //    // Inventory Limit
        //    // TODO: Noch nicht persistiert

        //    // Inventory
        //    int inventoryCount = reader.ReadInt32();
        //    for (int i = 0; i < inventoryCount; i++)
        //    {
        //        string definitionName = reader.ReadString();
        //        decimal amount = reader.ReadDecimal();

        //        var definition = definitionManager.GetItemDefinitions().FirstOrDefault(d => d.GetType().FullName.Equals(definitionName));
        //        if (definition != null)
        //        {
        //            InventorySlot slot = new InventorySlot();
        //            slot.Definition = definition;
        //            slot.Amount = amount;
        //            Inventory.Add(slot);
        //        }
        //    }

        //    // Inventory Tools (Index auf Inventory)
        //    byte toolCount = reader.ReadByte();
        //    for (byte i = 0; i < toolCount; i++)
        //    {
        //        byte index = reader.ReadByte();
        //        string definitionType = reader.ReadString();

        //        InventorySlot slot = Inventory.FirstOrDefault(s => s.Definition.GetType().FullName.Equals(definitionType));
        //        if (slot != null)
        //            Tools[index] = slot;
        //    }
        //}
    }
}
