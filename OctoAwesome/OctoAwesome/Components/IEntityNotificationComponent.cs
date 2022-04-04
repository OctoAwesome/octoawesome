using OctoAwesome.Notifications;

namespace OctoAwesome.Components
{

    public interface IEntityNotificationComponent : IEntityComponent, INotificationSubject<SerializableNotification>
    {
    }
}
