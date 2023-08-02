using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization;
internal class ComponentChangedNotificationHandler
{
    public Simulation? AssociatedSimulation { get; set; }

    private readonly IUpdateHub updateHub;
    private readonly ILogger logger;
    private readonly Dictionary<string, Action<Entity, EntityNotification, PropertyChangedNotification>> actionHandlers;

    public ComponentChangedNotificationHandler(IUpdateHub updatehub, ILogger logger)
    {
        actionHandlers = new()
        {
            { "PositionComponent", PositionChanged }, //TODO Move into own sth.
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Register(string key, Action<Entity, EntityNotification, PropertyChangedNotification> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(actionHandlers, key);
        if (val is null)
        {
            val = action;
        }
        else
        {
            val += action;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Unregister(string key, Action<Entity, EntityNotification, PropertyChangedNotification> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(actionHandlers, key);
        if (val is not null)
        {
            val -= action;
        }
    }

    private void AnimationChanged(Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = GetComponent<AnimationComponent>(entity, propertyChangedNotification.ComponentId);
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
    }


    private void InventoryChanged(Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var servcomp = entity.GetComponent<ServerManagedComponent>();
        var comp = GetComponent<InventoryComponent>(entity, propertyChangedNotification.ComponentId);
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
        if (servcomp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }

    private void PositionChanged(Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = entity.GetComponent<ServerManagedComponent>();
        var posComp = GetComponent<PositionComponent>(entity, propertyChangedNotification.ComponentId);
        if (posComp is null)
            return;

        _ = Serializer.Deserialize(posComp, propertyChangedNotification.Value);

        if (comp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }
    private T? GetComponent<T>(Entity e, int componentId) where T : IComponent
    {
        return AssociatedSimulation is not null
            ? AssociatedSimulation.GlobalComponentList.Get<T>(componentId)
            : e.GetComponent<T>(componentId);
    }
}
