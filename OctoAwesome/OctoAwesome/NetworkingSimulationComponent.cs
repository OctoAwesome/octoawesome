using engenious;

using NonSucking.Framework.Extension.Collections;

using OctoAwesome.Rx;
using OctoAwesome.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.AccessControl;
using System.IO;

namespace OctoAwesome
{


    public class GlobalComponentContainer : ComponentList<IComponent>
    {
        public static GlobalComponentContainer Instance { get; } = new GlobalComponentContainer();
    }

    public class NetworkingSimulationComponent
    {
        EnumerationModifiableConcurrentList<IComponent> dirties = new();
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IUpdateHub updateHub;
        private readonly IPool<PropertyChangedNotification> propertyChangedNotificationPool;

        public NetworkingSimulationComponent(IUpdateHub uh, IPool<EntityNotification> poolEn, IPool<PropertyChangedNotification> poolPcn)
        {
            entityNotificationPool = poolEn;
            updateHub = uh;
            propertyChangedNotificationPool = poolPcn;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in dirties)
            {
                if (item.Sendable
                    && item.Parent is Entity e)
                {
                    SendComponent(e, item);
                }
                dirties.Remove(item);
            }
        }

        public void Add(IComponent clean)
        {
            dirties.Add(clean);
        }

        public void Remove(IComponent cleanOrDirty)
        {
            dirties.Remove(cleanOrDirty);
        }

        private void SendComponent(Entity entity, IComponent component)
        {
            var updateNotification = propertyChangedNotificationPool.Rent();

            updateNotification.Issuer = component.GetType().SerializationId();
            updateNotification.ComponentId = component.Id;
            updateNotification.Value = Serializer.Serialize(component).ToArray();

            var entityNotification = entityNotificationPool.Rent();
            entityNotification.Entity = entity;
            entityNotification.Type = EntityNotification.ActionType.Update;
            entityNotification.Notification = updateNotification;

            updateHub.PushNetwork(entityNotification, DefaultChannels.Simulation);
            entityNotification.Release();
        }
    }
}
