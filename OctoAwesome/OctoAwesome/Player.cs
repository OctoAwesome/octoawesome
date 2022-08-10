using engenious;

using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Entity, that the user can control using input devices.
    /// </summary>
    [SerializationId(1, 1)]
    public sealed class Player : Entity
    {
        /// <summary>
        /// The range the user can interact with in game elements e.g. <see cref="Block"/> and <see cref="Entity"/>.
        /// </summary>
        public const int SELECTIONRANGE = 8;

        private readonly IPool<EntityNotification> entityNotificationPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
        }

        /// <inheritdoc />
        public override void OnNotification(SerializableNotification notification)
        {
            base.OnNotification(notification);

            var entityNotification = entityNotificationPool.Rent();
            entityNotification.Entity = this;
            entityNotification.Type = EntityNotification.ActionType.Update;
            entityNotification.Notification = notification as PropertyChangedNotification;

            Simulation?.OnUpdate(entityNotification);
            entityNotification.Release();
        }

        /// <inheritdoc/>
        protected override void OnInteract(GameTime gameTime, Entity entity) => throw new System.NotImplementedException();
    }
}
