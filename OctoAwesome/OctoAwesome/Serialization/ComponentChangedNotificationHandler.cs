using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization;
internal class ComponentChangedNotificationHandler
{
    private readonly IUpdateHub updateHub;
    private readonly ILogger logger;
    Dictionary<string, Action<Entity, EntityNotification, PropertyChangedNotification>> actionHandlers;

    public ComponentChangedNotificationHandler(IUpdateHub uh, ILogger logger)
    {
        actionHandlers = new();
        actionHandlers.Add("PositionComponent", PositionChanged);
        updateHub = uh;
        this.logger = logger.As(typeof(ComponentChangedNotificationHandler));
    }

    public void Abc(Entity entity, EntityNotification entityNot)
    {
        if (entityNot.Notification is not PropertyChangedNotification propChanged)
        {
            return;
        }
        if (actionHandlers.TryGetValue(propChanged.Issuer, out var handler))
        {
            handler.Invoke(entity, entityNot, propChanged);
        }


    }
    private void PositionChanged(Entity instance, EntityNotification notification, PropertyChangedNotification propNot)
    {
        var comp = instance.GetComponent<ServerManagedComponent>();
        var posComp = instance.GetComponent<PositionComponent>();
        if (posComp is null)
            return;

        _ = Serializer.Deserialize(posComp, propNot.Value);

        if (comp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }
}
