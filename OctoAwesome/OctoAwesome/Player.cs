using System.Collections.Generic;
using System.Xml.Serialization;
using engenious;
using System.IO;
using System.Linq;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;

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
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() : base()
        {
        }

        protected override void OnInitialize(IResourceManager manager)
        {
            //Cache = new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);
        }

        /// <summary>
        /// Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public override void Serialize(BinaryWriter writer)
            => base.Serialize(writer); // Entity

        /// <summary>
        /// Deserialisiert den Player aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public override void Deserialize(BinaryReader reader)
            => base.Deserialize(reader); // Entity

        public override void OnUpdate(SerializableNotification notification)
        {
            base.OnUpdate(notification);

            var entityNotification = new EntityNotification
            {
                Entity = this,
                Type = EntityNotification.ActionType.Update,
                Notification = notification as PropertyChangedNotification
            };

            Simulation?.OnUpdate(entityNotification);
        }
        
    }
}
