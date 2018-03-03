using System.IO;
using engenious;
using OctoAwesome.Common;
using OctoAwesome.Entities;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    public sealed class Player : Entity, IControllable, Entities.IDrawable
    {
        /// <summary>
        /// Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block"/> und <see cref="Entity"/> interagieren kann
        /// </summary>
        public const int SELECTIONRANGE = 8;
        /// <summary>
        /// Current controller of the Player.
        /// </summary>
        public IEntityController Controller { get; private set; }

        public string Name { get; set; }

        public string ModelName { get; set; }

        public string TextureName { get; set; }

        public float BaseRotationZ { get; set; }

        public float Height { get; set; }

        public float Radius { get; set; }

        public bool DrawUpdate { get; set; }
        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player() : base(false)
        {
            Height = 3.5f;
            Radius = 0.8f;
        }
        /// <summary>
        /// Register a Controller.
        /// </summary>
        /// <param name="controller"></param>
        public void Register(IEntityController controller)
        {
            Controller = controller;
        }
        /// <summary>
        /// Reset the controller to deault.
        /// </summary>
        public void Reset()
        {
            Controller = null;
        }
        /// <summary>
        /// Called during initialize.
        /// </summary>
        /// <param name="service"><see cref="IGameService"/></param>
        protected override void OnInitialize(IGameService service)
        {
            Cache = service.GetLocalCache(false, 2, 1);
            //SetPosition(new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)), 0);
        }
        /// <summary>
        /// Serialisiert den Player mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public override void Serialize(BinaryWriter writer, IDefinitionManager definition)
            => base.Serialize(writer, definition); // Entity
        /// <summary>
        /// Deserialisiert den Player aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IGameService"/>.</param>
        public override void Deserialize(BinaryReader reader, IDefinitionManager definition)
            => base.Deserialize(reader, definition); // Entity
    }
}
