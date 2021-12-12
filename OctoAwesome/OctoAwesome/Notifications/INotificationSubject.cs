namespace OctoAwesome.Notifications
{
    public interface INotificationSubject<in TNotification> where TNotification : Notification
    {
        void OnNotification(TNotification notification);

        void Push(TNotification notification);
    }
}
