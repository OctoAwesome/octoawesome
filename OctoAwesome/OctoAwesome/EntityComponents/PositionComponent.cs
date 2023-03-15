using engenious;

using OctoAwesome.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using NonSucking.Framework.Serialization;

using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for entities with an position.
    /// </summary>
    //[NoosonCustom(SerializeMethodName = nameof(Serialize), DeserializeMethodName = nameof(Deserialize))]
    [Nooson]
    public sealed partial class PositionComponent : InstanceComponent<ComponentContainer>, IEntityComponent, ISerializable<PositionComponent>
    {
        /// <summary>
        /// Gets or sets the position of the entity.
        /// </summary>
        [NoosonIgnore]
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
        [NoosonOrder(1)]
        public float Direction { get; set; }
        /// <summary>
        /// Gets the planet the entity is on.
        /// </summary>
        [NoosonIgnore]
        public IPlanet Planet => planet ??= TryGetPlanet(position.Planet);
        [NoosonInclude, NoosonOrder(0)]
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
        //public override void Serialize(BinaryWriter writer)
        //{
        //    base.Serialize(writer);
        //    // Position
        //    writer.Write(Position.Planet);
        //    writer.Write(Position.GlobalBlockIndex.X);
        //    writer.Write(Position.GlobalBlockIndex.Y);
        //    writer.Write(Position.GlobalBlockIndex.Z);
        //    writer.Write(Position.BlockPosition.X);
        //    writer.Write(Position.BlockPosition.Y);
        //    writer.Write(Position.BlockPosition.Z);
        //}

        ///// <inheritdoc />
        //public static PositionComponent Deserialize(BinaryReader reader)
        //{
        //    var posCom = new PositionComponent();
        //    ((InstanceComponent<ComponentContainer>) posCom).Deserialize(reader);

        //    // Position
        //    int planet = reader.ReadInt32();
        //    int blockX = reader.ReadInt32();
        //    int blockY = reader.ReadInt32();
        //    int blockZ = reader.ReadInt32();
        //    float posX = reader.ReadSingle();
        //    float posY = reader.ReadSingle();
        //    float posZ = reader.ReadSingle();

        //    posCom.position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
        //    return posCom;
        //}

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

                updateNotification.Value = Serializer.Serialize(this);

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
                        _ = Serializer.Deserialize(this, changedNotification.Value);
                    }
                }
            }
        }
    }
}
