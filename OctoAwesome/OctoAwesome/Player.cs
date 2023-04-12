using engenious;

using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using NonSucking.Framework.Serialization;
using OctoAwesome.EntityComponents;
using OctoAwesome.Components;
using System.IO;
using OctoAwesome.Extension;

namespace OctoAwesome
{
   

    /// <summary>
    /// Entity, that the user can control using input devices.
    /// </summary>
    [SerializationId(1, 1)]
    [Nooson]
    public partial class Player : Entity, IConstructionSerializable<Player>
    {
        /// <summary>
        /// The range the user can interact with in game elements e.g. <see cref="Block"/> and <see cref="Entity"/>.
        /// </summary>
        public const int SELECTIONRANGE = 8;

        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IUpdateHub updateHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            updateHub = TypeContainer.Get<IUpdateHub>();
        }


        /// <inheritdoc/>
        protected override void OnInteract(GameTime gameTime, Entity entity) => throw new System.NotImplementedException();

    }
}
