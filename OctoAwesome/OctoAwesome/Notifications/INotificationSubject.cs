namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Interface for receiving notifications of a specific subject type.
    /// </summary>
    /// <typeparam name="TNotification">The subject notification type.</typeparam>
    public interface INotificationSubject<in TNotification> where TNotification : Notification
    {
        /// <summary>
        /// Gets called when a notification is received.
        /// </summary>
        /// <param name="notification">The notification that was received.</param>
        void OnNotification(TNotification notification);

        /// <summary>
        /// Push a new notification to be handled.
        /// </summary>
        /// <param name="notification">The new notification to be handled.</param>
        void Push(TNotification notification);
    }
}
