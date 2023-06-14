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

    public ComponentChangedNotificationHandler(IUpdateHub updatehub, ILogger logger)
    {
        actionHandlers = new()
        {
            { "PositionComponent", PositionChanged } , //TODO Move into own sth.
            { "AnimationComponent", AnimationChanged },
            { "InventoryComponent", InventoryChanged },
        };
        updateHub = updatehub;
        this.logger = logger.As(typeof(ComponentChangedNotificationHandler));
    }


    public void Execute(Entity entity, EntityNotification entityNotitification)
    {
        if (entityNotitification.Notification is not PropertyChangedNotification propChanged)
        {
            return;
        }
        logger.Trace($"Rec {nameof(EntityNotification)} of entity {entity.Id} with type {propChanged.Issuer}");
        if (actionHandlers.TryGetValue(propChanged.Issuer, out var handler))
        {
            handler.Invoke(entity, entityNotitification, propChanged);
        }
    }

    private void AnimationChanged(Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = entity.GetComponent<AnimationComponent>();
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
    }
    private void InventoryChanged(Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var servcomp = entity.GetComponent<ServerManagedComponent>();
        var comp = entity.GetComponent<InventoryComponent>();
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
        if (servcomp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }

    private void PositionChanged(Entity instance, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = instance.GetComponent<ServerManagedComponent>();
        var posComp = instance.GetComponent<PositionComponent>();
        if (posComp is null)
            return;

        _ = Serializer.Deserialize(posComp, propertyChangedNotification.Value);

        if (comp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }
}
