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

                posUpdate = valueBlockX != positionBlockX 
                    || valueBlockY != positionBlockY
                    || position.BlockPosition.Z != value.BlockPosition.Z;
                
                position =  value;
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
        }


        private IPlanet TryGetPlanet(int planetId)
        {
            if (planet != null && planet.Id == planetId)
                return planet;

            return resourceManager.GetPlanet(planetId);
        }
    }
}
