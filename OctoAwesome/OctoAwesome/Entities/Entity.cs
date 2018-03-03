using engenious;
using OctoAwesome.Common;
using System.IO;
namespace OctoAwesome.Entities
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity : ISerializable
    {
        /// <summary>
        /// Indicates that the <see cref="Entity"/> need an Update.
        /// </summary>
        public bool NeedUpdate { get; private set; }
        /// <summary>
        /// Horizontaler winkel.
        /// </summary>
        public float Azimuth { get; private set; }
        /// <summary>
        /// <see cref="Coordinate"/> of the <see cref="Entity"/>.
        /// </summary>
        public Coordinate Position { get; private set; }
        /// <summary>
        /// Contains all Components.
        /// </summary>
        public ComponentList<EntityComponent> Components { get; private set; }
        /// <summary>
        /// Temp Id
        /// </summary>
        public int Id { get; internal set; }
        /// <summary>
        /// LocalChunkCache für die Entity
        /// </summary>
        public ILocalChunkCache Cache { get; protected set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Entity() : this(false)
        {

        }
        /// <summary>
        /// Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        /// <param name="needUpdate">Indicates that <see cref="Entity"/> need an Update.</param>
        public Entity(bool needUpdate)
        {
            NeedUpdate = needUpdate;
            Components = new ComponentList<EntityComponent>(OnAddComponent, OnRemoveComponent);
        }
        /// <summary>
        /// Updatemethod for the <see cref="Entity"/>
        /// </summary>
        /// <param name="gameTime">Time of the Simulation.</param>
        /// <param name="service">Game services</param>
        internal void Update(GameTime gameTime, IGameService service)
        {
            OnUpdate(gameTime, service);
        }
        /// <summary>
        /// Called dirung Update of the <see cref="Entity"/>
        /// </summary>
        /// <param name="gameTime">Time of the Simulation.</param>
        /// <param name="service">Game services</param>
        protected virtual void OnUpdate(GameTime gameTime, IGameService service)
        {

        }
        /// <summary>
        /// Set the Postion of the <see cref="Entity"/>
        /// </summary>
        /// <param name="position">The new Position</param>
        /// <param name="azimuth">Horizontal angle</param>
        public void SetPosition(Coordinate position, float azimuth = 0)
        {
            if(Cache != null)
                Cache.SetCenter(Cache.Planet, new Index2(Position.ChunkIndex));

            OnSetPosition(position, azimuth);
            Position = position;
            Azimuth = azimuth;
        }
        /// <summary>
        /// Called during SetPosition (before Position is set)
        /// </summary>
        /// <param name="position">New Position</param>
        /// <param name="azimuth">Horizontal angle</param>
        protected virtual void OnSetPosition(Coordinate position, float azimuth)
        {

        }
        /// <summary>
        /// Initialize the Entity.
        /// </summary>
        /// <param name="service">Game Service</param>
        public void Initialize(IGameService service)
        {
            OnInitialize(service);
        }
        /// <summary>
        /// Called during initialize.
        /// </summary>
        /// <param name="service"><see cref="IGameService"/></param>
        protected virtual void OnInitialize(IGameService service)
        {
        }
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definition)
        {
            writer.Write(NeedUpdate);
            writer.Write(Azimuth);
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
            Components.Serialize(writer, definition);
        }
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definition)
        {
            NeedUpdate = reader.ReadBoolean();
            Azimuth = reader.ReadSingle();
            Position = new Coordinate(reader.ReadInt32(),
                new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            Components.Deserialize(reader, definition);
        }
        private void OnRemoveComponent(EntityComponent component)
        {
            //if (Simulation != null)
            //    throw new NotSupportedException("Can't remove components during simulation");

        }
        private void OnAddComponent(EntityComponent component)
        {
            //if (Simulation != null)
            //    throw new NotSupportedException("Can't add components during simulation");
            component.SetEntity(this);
        }
    }
}
