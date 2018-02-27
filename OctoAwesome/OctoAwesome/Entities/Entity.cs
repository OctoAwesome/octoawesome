using engenious;
using engenious.Helper;
using System;
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
        public bool NeedUpdate { get; }
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
        ///// <summary>
        ///// Reference to the active Simulation.
        ///// </summary>
        //public Simulation Simulation { get; internal set; }
        /// <summary>
        /// LocalChunkCache für die Entity
        /// </summary>
        public ILocalChunkCache Cache { get; protected set; }
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
        public virtual void Update(GameTime gameTime)
        {

        }
        /// <summary>
        /// Register default for this entity
        /// </summary>
        public void RegisterDefault()
        {

        }
        /// <summary>
        /// Move the <see cref="Entity"/>
        /// </summary>
        /// <param name="moved">Value of position change</param>
        /// <param name="azimuth">Horizontal angle</param>
        public void Move(Vector3 moved, float azimuth = 0)
        {
            Coordinate position = Position + moved;
            position.NormalizeChunkIndexXY(Cache.Planet.Size);
            SetPosition(position, azimuth);
        }
        /// <summary>
        /// Set the Postion of the <see cref="Entity"/>
        /// </summary>
        /// <param name="position">The new Position</param>
        /// <param name="ignorecenter">Ignore SetCenter call on localcache</param>
        /// <param name="azimuth">Horizontal angle</param>
        public void SetPosition(Coordinate position, float azimuth = 0, bool ignorecenter = false)
        {
            if (ignorecenter || Cache != null && Cache.SetCenter(Cache.Planet, new Index2(Position.ChunkIndex)))
            {
                OnSetPosition(position);
                Position = position;
            }
        }
        /// <summary>
        /// Called during SetPosition (before Position is set)
        /// </summary>
        /// <param name="position">New Position</param>
        protected virtual void OnSetPosition(Coordinate position)
        {

        }
        /// <summary>
        /// Initialize the Entity.
        /// </summary>
        /// <param name="mananger"></param>
        public void Initialize(IResourceManager mananger)
        {
            OnInitialize(mananger);
        }
        /// <summary>
        /// Called during initialization.
        /// </summary>
        /// <param name="manager"></param>
        protected virtual void OnInitialize(IResourceManager manager)
        {
        }
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            Components.Serialize(writer, definitionManager);
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
            writer.Write(Position.ChunkIndex.X);
            writer.Write(Position.ChunkIndex.Y);
            writer.Write(Position.ChunkIndex.Z);
        }
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            Components.Deserialize(reader, definitionManager);
            Position = new Coordinate(reader.ReadInt32(),
                new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
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
        }
    }
}
