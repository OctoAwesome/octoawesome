using System;

namespace OctoAwesome.Ecs
{
    public class EventSubscription<TComponent, TEvent> : EventSubscription
        where TComponent : Component, new()
        where TEvent : GameEvent
    {
        private readonly Action<EntityManager, Entity, TComponent, TEvent> _callBack;

        public EventSubscription(Action<EntityManager, Entity, TComponent, TEvent> cb, int priority)
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
            _callBack(manager, e, e.Get<TComponent>(), (TEvent)@event);
            return true;
        }
    }

    public abstract class EventSubscription : IComparable<EventSubscription>
    {
        public int Priority;

        public int CompareTo(EventSubscription other)
        {
            return Priority.CompareTo(other.Priority);
        }

        public abstract bool Matches(Entity e);
        public abstract bool Invoke(EntityManager manager, Entity e, object @event);
    }
}