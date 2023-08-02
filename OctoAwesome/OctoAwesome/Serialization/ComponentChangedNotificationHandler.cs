using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization;
internal class ComponentChangedNotificationHandler
{
    public Simulation? AssociatedSimulation { get; set; }

    private readonly IUpdateHub updateHub;
    private readonly ILogger logger;
    private readonly Dictionary<string, Action<ComponentChangedNotificationHandler, Entity, EntityNotification, PropertyChangedNotification>> actionHandlers;

    public ComponentChangedNotificationHandler(IUpdateHub updatehub, ILogger logger)
    {
        actionHandlers = new()
        {
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
            handler.Invoke(this, entity, entityNotitification, propChanged);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action">Name of the Class to interact with, first component container is the interactor and second component container the target</param>
    public void Register(string key, Action<ComponentChangedNotificationHandler, Entity, EntityNotification, PropertyChangedNotification> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(actionHandlers, key);
        if (Unsafe.IsNullRef(ref val))
        {
            actionHandlers[key] = action;
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
    public void Unregister(string key, Action<ComponentChangedNotificationHandler, Entity, EntityNotification, PropertyChangedNotification> action)
    {
        ref var val = ref CollectionsMarshal.GetValueRefOrNullRef(actionHandlers, key);
        if (!Unsafe.IsNullRef(ref val))
        {
            val -= action;
        }
    }

    /// <summary>
    /// Gets the component from the <see cref="AssociatedSimulation"/> or the <see cref="Entity"/> if <see cref="AssociatedSimulation"/> is null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    /// <param name="componentId"></param>
    /// <returns></returns>
    public T? GetComponent<T>(Entity e, int componentId) where T : IComponent
    {
        return AssociatedSimulation is not null
            ? AssociatedSimulation.GlobalComponentList.Get<T>(componentId)
            : e.GetComponent<T>(componentId);
    }
}

internal class ComponentChangeContainer
{
    private readonly IUpdateHub updateHub;

    public ComponentChangeContainer(IUpdateHub updateHub)
    {
        this.updateHub = updateHub;
    }

    internal void AnimationChanged(ComponentChangedNotificationHandler handler, Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = handler.GetComponent<AnimationComponent>(entity, propertyChangedNotification.ComponentId);
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
    }


    internal void InventoryChanged(ComponentChangedNotificationHandler handler, Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var servcomp = entity.GetComponent<ServerManagedComponent>();
        var comp = handler.GetComponent<InventoryComponent>(entity, propertyChangedNotification.ComponentId);
        if (comp is null)
            return;

        _ = Serializer.Deserialize(comp, propertyChangedNotification.Value);
        if (servcomp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }

    internal void PositionChanged(ComponentChangedNotificationHandler handler, Entity entity, EntityNotification notification, PropertyChangedNotification propertyChangedNotification)
    {
        var comp = entity.GetComponent<ServerManagedComponent>();
        var posComp = handler.GetComponent<PositionComponent>(entity, propertyChangedNotification.ComponentId);
        if (posComp is null)
            return;

        _ = Serializer.Deserialize(posComp, propertyChangedNotification.Value);

        if (comp is { OnServer: true })
        {
            updateHub.PushNetwork(notification, DefaultChannels.Simulation);
        }
    }
}
