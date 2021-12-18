using engenious;
using OctoAwesome.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for entities with an position.
    /// </summary>
    public sealed class PositionComponent : InstanceComponent<ComponentContainer>, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Gets or sets the position of the entity.
        /// </summary>
        public Coordinate Position
        {
            get => position;
            set
            {
                var valueBlockX = ((int)(value.BlockPosition.X * 100)) / 100f;
                var valueBlockY = ((int)(value.BlockPosition.Y * 100)) / 100f;
                var positionBlockX = ((int)(position.BlockPosition.X * 100)) / 100f;
                var positionBlockY = ((int)(position.BlockPosition.Y * 100)) / 100f;

                posUpdate = valueBlockX != positionBlockX || valueBlockY != positionBlockY
                    || position.BlockPosition.Z != value.BlockPosition.Z;

                SetValue(ref position, value);
                planet = TryGetPlanet(value.Planet);
            }
        }

        /// <summary>
        /// Gets or sets the direction the entity is facing.
        /// </summary>
        public float Direction { get; set; }
        /// <summary>
        /// Gets the planet the entity is on.
        /// </summary>
        public IPlanet Planet => planet ??= TryGetPlanet(position.Planet);

        private Coordinate position;
        private bool posUpdate;
        private IPlanet? planet;
        private readonly IResourceManager resourceManager;
        private readonly IPool<PropertyChangedNotification> propertyChangedNotificationPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionComponent"/> class.
        /// </summary>
        public PositionComponent()
        {
            Sendable = true;
            resourceManager = TypeContainer.Get<IResourceManager>();
            propertyChangedNotificationPool = TypeContainer.Get<IPool<PropertyChangedNotification>>();
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            // Position
            int planet = reader.ReadInt32();
            int blockX = reader.ReadInt32();
            int blockY = reader.ReadInt32();
            int blockZ = reader.ReadInt32();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float posZ = reader.ReadSingle();

            position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
        }

        private IPlanet TryGetPlanet(int planetId)
        {
            if (planet != null && planet.Id == planetId)
                return planet;

            return resourceManager.GetPlanet(planetId);
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged<T>(T value, string propertyName)
        {
            base.OnPropertyChanged(value, propertyName);

            if (propertyName == nameof(Position) && posUpdate)
            {
                var updateNotification = propertyChangedNotificationPool.Rent();

                updateNotification.Issuer = nameof(PositionComponent);
                updateNotification.Property = propertyName;

                using (var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    Serialize(writer);
                    updateNotification.Value = stream.ToArray();
                }

                Push(updateNotification);
            }
        }

        /// <inheritdoc />
        public override void OnNotification(SerializableNotification notification)
        {
            base.OnNotification(notification);

            if (notification is PropertyChangedNotification changedNotification)
            {
                if (changedNotification.Issuer == nameof(PositionComponent))
                {
                    if (changedNotification.Property == nameof(Position))
                    {
                        using (var stream = new MemoryStream(changedNotification.Value))
                        using (var reader = new BinaryReader(stream))
                        {
                            Deserialize(reader);
                        }
                    }
                }
            }
        }
    }
}
