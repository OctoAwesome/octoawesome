using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity : ISerializable, IIdentification
    {
        /// <summary>
        /// Contains all Components.
        /// </summary>
        public ComponentList<EntityComponent> Components { get; private set; }

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Reference to the active Simulation.
        /// </summary>
        public Simulation Simulation { get; internal set; }


        /// <summary>
        /// Entity die regelmäßig eine Updateevent bekommt
        /// </summary>
        public Entity()
        {
            Components = new ComponentList<EntityComponent>(
                ValidateAddComponent, ValidateRemoveComponent, OnAddComponent, OnRemoveComponent);
        }

        private void OnRemoveComponent(EntityComponent component)
        {

        }

        private void OnAddComponent(EntityComponent component)
        {
            component.SetEntity(this);

            //HACK: Remove PositionComponent Dependency
            if (component is LocalChunkCacheComponent cacheComponent)
            {
                if (cacheComponent.LocalChunkCache != null)
                    return;

                var positionComponent = Components.GetComponent<PositionComponent>();

                if (positionComponent == null)
                    return;

                cacheComponent.LocalChunkCache = new LocalChunkCache(positionComponent.Planet.GlobalChunkCache, 4, 2);
            }
        }

        private void ValidateAddComponent(EntityComponent component)
        {
            if (Simulation != null)
                throw new NotSupportedException("Can't add components during simulation");
        }

        private void ValidateRemoveComponent(EntityComponent component)
        {
            if (Simulation != null)
                throw new NotSupportedException("Can't remove components during simulation");
        }

        public void Initialize(IResourceManager mananger)
        {
            OnInitialize(mananger);
        }

        protected virtual void OnInitialize(IResourceManager manager)
        {
        }

        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);

            Components.Serialize(writer);
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Components.Deserialize(reader);
        }

        public virtual void RegisterDefault()
        {

        }

        public override int GetHashCode()
            => Id;

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
                return entity.Id == Id;

            return base.Equals(obj);
        }

        public virtual void OnUpdate(SerializableNotification notification)
        {
        }


        public virtual void Update(SerializableNotification notification)
        {
            foreach (var component in Components)
                component?.OnUpdate(notification);
        }

    }
}
