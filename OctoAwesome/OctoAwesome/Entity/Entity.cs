using engenious;
using OctoAwesome.EntityComponents;
using System;
using System.IO;

namespace OctoAwesome.Entity
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Mass of the Body.
        /// </summary>
        public float Mass { get; set; }
        /// <summary>
        /// Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }
        /// <summary>
        /// Die Körperhöhe des Spielers in Blocks.
        /// </summary>
        public float Height { get; set; }
        /// <summary>
        /// Position of the Entity in the World.
        /// </summary>
        public Coordinate Position { get; set; }
        /// <summary>
        /// Azimuth of the Entity in the World.
        /// </summary>
        public float Azimuth { get; set; }
        /// <summary>
        /// Indicator for Update requierment.
        /// </summary>
        public bool NeedUpdate { get; private set; }
        /// <summary>
        /// Contains all Components.
        /// </summary>
        public ComponentList<EntityComponent> Components { get; private set; }
        /// <summary>
        /// Temp Id
        /// </summary>
        public int Id { get; internal set; }
        /// <summary>
        /// Reference to the active Simulation.
        /// </summary>
        public Simulation Simulation { get; internal set; }
        /// <summary>
        /// LocalChunkCache für die Entity
        /// </summary>
        public ILocalChunkCache Cache { get; protected set; }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Entity() : this(false)
        {
        }
        /// <summary>
        /// Constructor with update indicator.
        /// </summary>
        /// <param name="needUpdate">Indicates that the Entity need updates.</param>
        public Entity(bool needUpdate)
        {
            NeedUpdate = NeedUpdate;
            Components = new ComponentList<EntityComponent>(OnAddComponent, OnRemoveComponent);
        }        
        /// <summary>
        /// Update the Entity.
        /// </summary>
        /// <param name="gameTimte">Simulaiton time.</param>
        public virtual void Update(GameTime gameTimte)
        {

        }
        /// <summary>
        /// Register default components for the Entity.
        /// </summary>
        public virtual void RegisterDefault()
        {
        }
        /// <summary>
        /// Initialze the Entity.
        /// </summary>
        /// <param name="mananger"></param>
        public void Initialize(IResourceManager mananger)
        {
            OnInitialize(mananger);
        }
        /// <summary>
        /// Is called during <see cref="Initialize(IResourceManager)"/>.
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
            writer.Write(NeedUpdate);
            // Body
            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
            // Position
            writer.Write(Azimuth);
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
            // Components
            Components.Serialize(writer, definitionManager);
        }
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            NeedUpdate = reader.ReadBoolean();
            // Body
            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
            // Position
            Azimuth = reader.ReadSingle();
            Position = new Coordinate(reader.ReadInt32(),
                new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            // Components
            Components.Deserialize(reader, definitionManager);
        }
        private void OnRemoveComponent(Component component)
        {
            // TODO: wie sinnvoll ist das ? -> wenn das passiert ist es ein architektur fehler^^
            if (Simulation != null)
                throw new NotSupportedException("Can't add components during simulation");
        }
        private void OnAddComponent(Component component)
        {
            // TODO: wie sinnvoll ist das ? -> wenn das passiert ist es ein architektur fehler^^
            if (Simulation != null)
                throw new NotSupportedException("Can't remove components during simulation");
        }
    }
}
