using System.IO;
using OctoAwesome.Entities;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : Entity, IControllable
    {
        /// <summary>
        /// Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block"/> und <see cref="Entity"/> interagieren kann
        /// </summary>
        public const int SELECTIONRANGE = 8;
        private IController currentcontroller;

        public IController Controller => currentcontroller;

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() : base(false)
        {
        }

        protected override void OnInitialize(IResourceManager manager)
            => Cache = new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);

        /// <summary>
        /// Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
            => base.Serialize(writer, definitionManager); // Entity

        /// <summary>
        /// Deserialisiert den Player aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
            => base.Deserialize(reader, definitionManager); // Entity

        public void Register(IController controller)
        {
            currentcontroller = controller;
        }

        public void Reset()
        {
            currentcontroller = null;
        }
    }
}
