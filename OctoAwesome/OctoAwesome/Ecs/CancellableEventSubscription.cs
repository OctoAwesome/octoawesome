using System;

namespace OctoAwesome.Ecs
{
    public class CancellableEventSubscription<TComponent, TEvent> : EventSubscription
        where TComponent : Component, new()
        where TEvent : GameEvent
    {
        private readonly Func<EntityManager, Entity, TComponent, TEvent, bool> _callBack;

        public CancellableEventSubscription(Func<EntityManager, Entity, TComponent, TEvent, bool> cb, int priority)
        {
            _callBack = cb;
            Priority = priority;
        }

        public override bool Matches(Entity e)
        {
            return e.Get<TComponent>() != null;
        }

        public override bool Invoke(EntityManager manager, Entity e, object @event)
        {
            return _callBack(manager, e, e.Get<TComponent>(), (TEvent)@event);
        }
    }
}