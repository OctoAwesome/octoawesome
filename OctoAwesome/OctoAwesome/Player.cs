using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Entität, die der menschliche Spieler mittels Eingabegeräte steuern kann.
    /// </summary>
    [SerializationId(1, 1)]
    public sealed class Player : Entity
    {
        /// <summary>
        /// Die Reichweite des Spielers, in der er mit Spielelementen wie <see cref="Block"/> und <see cref="Entity"/> interagieren kann
        /// </summary>
        public const int SELECTIONRANGE = 8;

        private readonly IPool<EntityNotification> entityNotificationPool;

        /// <summary>
        /// Erzeugt eine neue Player-Instanz an der Default-Position.
        /// </summary>
        public Player()
        {
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
        }
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

    }
}
