using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using engenious;

namespace OctoAwesome.Ecs
{
    public class EntityReference
    {
        public Entity Entity;
    }

    public class ComponentConfigAttribute : Attribute
    {
        public int Prefill;

        public ComponentConfigAttribute(int prefill = 16)
        {
            Prefill = prefill;
        }
    }
    public class GameEvent { }
    public class EventRegistry<TEvent> where TEvent : GameEvent
    {
        // ReSharper disable once StaticMemberInGenericType
        public static int Index;

        public static void Initialize(int index)
        {
            Index = index;
        }
    }


    public class EntityManager
    {
        private readonly List<Entity> _addedEntities = new List<Entity>();
        private readonly List<Entity> _changedEntities = new List<Entity>();

        private readonly List<Entity> _entities = new List<Entity>();

        private readonly List<Action> _pendingActions = new List<Action>(512);

        private readonly Dictionary<string, Func<EntityManager, Entity>> _prefabs = new Dictionary<string, Func<EntityManager, Entity>>();
        private readonly List<Entity> _removedEntities = new List<Entity>();
        public readonly List<BaseSystem> Systems = new List<BaseSystem>();
        public readonly List<List<BaseSystem>> UpdateGroups = new List<List<BaseSystem>>();

        public static readonly Dictionary<string, Action<Entity, BinaryReader>> Deserializers;

        public static readonly int ComponentCount;
        private int _lastIndex = 0;

        private static readonly List<List<EventSubscription>> SubscriptionMap;
        
        static EntityManager()
        {
            var baseEventType = typeof(GameEvent);

            var eventTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && baseEventType.IsAssignableFrom(t))
                .ToList();

            var registryType = typeof(EventRegistry<>);
            for (var i = 0; i < eventTypes.Count; i++)
            {
                var eventType = eventTypes[i];
                var rType = registryType.MakeGenericType(eventType);

                rType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(
                    null,
                    new object[] {
                        i
                    });
            }

            SubscriptionMap = new List<List<EventSubscription>>(eventTypes.Count);

            for (var i = 0; i < eventTypes.Count; i++)
            {
                SubscriptionMap.Add(new List<EventSubscription>());
            }

            var baseComponentType = typeof(Component);
            var componentTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && baseComponentType.IsAssignableFrom(t))
                .ToList();

            registryType = typeof(ComponentRegistry<>);
            var entityType = typeof(Entity);
            
            ComponentCount = componentTypes.Count;
             Deserializers = new Dictionary<string, Action<Entity, BinaryReader>>(ComponentCount);
            ComponentArrayPool.Initialize(ComponentCount);

            for (var i = 0; i < componentTypes.Count; i++)
            {
                var componentType = componentTypes[i];
                var rType = registryType.MakeGenericType(componentType);

                var attr = componentType.GetCustomAttributes(typeof(ComponentConfigAttribute), false);

                int prefill = 16;
                if (attr.Length > 0)
                {
                    var a = (ComponentConfigAttribute)attr[0];
                    prefill = a.Prefill;
                }
                rType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Invoke(
                    null,
                    new object[] {
                        i,  prefill
                    });
                componentType.GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)?.Invoke(null, null);

                var deserializeMethod = componentType.GetMethod("Deserialize", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,null, new []{ typeof(Entity), componentType, typeof(BinaryReader) }, null);

                if (deserializeMethod == null)
                {
                    throw new Exception($"Component {componentType.FullName} has no valid deserialize method. Expected signature: static void Deserialize(Entity,{componentType.FullName},BinaryReader)");
                }

                var p1 = Expression.Parameter(typeof(Entity));
                var p3 = Expression.Parameter(typeof(BinaryReader));
                var variable = Expression.Variable(componentType);

                var getMethod = rType.GetMethod("Take", BindingFlags.Static | BindingFlags.Public);

                Deserializers[componentType.FullName] = Expression.Lambda<Action<Entity, BinaryReader>>(
                    Expression.Block(new [] { variable },
                       Expression.Assign(variable, Expression.Call(null, getMethod)),
                    Expression.Assign(Expression.ArrayAccess(Expression.Field(p1, typeof(Entity).GetField("Components")), Expression.Constant(i)), 
                        variable
                    ),
                    Expression.Call(deserializeMethod, p1, variable, p3)
                    ), 
                false, p1, p3).Compile();

            }

            foreach (var map in SubscriptionMap)
                map.Sort();
        }

        public void ApplyChanges()
        {
            if (_pendingActions.Count > 0)
            {
                foreach (var a in _pendingActions)
                    a();

                _pendingActions.Clear();
            }

            if (_addedEntities.Count > 0)
            {
                foreach (var r in _addedEntities)
                {
                    foreach (var s in Systems)
                        s.EntityAdded(r);
                }

                _entities.AddRange(_addedEntities);
                _addedEntities.ForEach(e => e.Complete = true);
                _addedEntities.Clear();
            }

            if (_changedEntities.Count > 0)
            {
                foreach (var r in _changedEntities)
                {
                    foreach (var s in Systems)
                        s.EntityChanged(r);
                }

                _changedEntities.Clear();
            }

            if (_removedEntities.Count > 0)
            {
                foreach (var r in _removedEntities)
                {
                    foreach(var s in Systems)
                        s.EntityRemoved(r);
                    ComponentArrayPool.Release(r.Components);
                    _entities.Remove(r);
                }
                _removedEntities.Clear();
            }
        }

        public GameTime GameTime;

        public void Update(GameTime gameTime)
        {

            GameTime = gameTime;
            ApplyChanges();
            Tick();
            ApplyChanges();
        }

        public void Tick()
        {
            Parallel.ForEach(UpdateGroups, g => g.ForEach(s => s.Tick()));
        }

        public EntityManager Add<T>(Entity e, bool throwOnExists = false) where T : Component, new()
        {
            var idx = FlagIndex<T>();
            if (e.Components[idx] == null)
            {
                e.Components[idx] = ComponentRegistry<T>.Take();

                if (e.Complete)
                    _changedEntities.Add(e);
            }
            else if (throwOnExists)
            {
                throw new InvalidOperationException($"{typeof(T)} exists on entity");
            }
            return this;
        }

        public EntityManager Add<T>(Entity e, Action<T> action, bool actionIfAlreadyExists = false, bool throwOnExists = false) where T : Component, new()
        {
            if (action == null)
                return Add<T>(e);

            var idx = FlagIndex<T>();

            var existing = e.Get<T>();

            if (existing == null)
            {
                var item = ComponentRegistry<T>.Take();
                action(item);
                e.Components[idx] = item;

                if (e.Complete)
                    _changedEntities.Add(e);
            }
            else if (throwOnExists)
            {
                throw new InvalidOperationException($"{typeof(T)} exists on entity");
            }
            else if (actionIfAlreadyExists)
            {
                action(existing);
            }
            
            return this;
        }

        public EntityManager Remove<T>(Entity e) where T : Component, new()
        {
            lock (_pendingActions)
            {
                _pendingActions.Add(
                    () => {
                        ComponentRegistry<T>.Release(e.Get<T>());
                        if (e.Complete)
                            _changedEntities.Add(e);
                    });
            }

            return this;
        }

        public Entity NewEntity()
        {
            var e = new Entity { Components = ComponentArrayPool.Take(), Manager = this };
            _addedEntities.Add(e);
            return e;
        }
       
        public int FlagIndex<T>() where T : Component, new()
        {
            return ComponentRegistry<T>.Index;
        }

        public void Publish<TEvent>(Entity e, TEvent @event) where TEvent : GameEvent
        {
            foreach (var sub in SubscriptionMap[EventRegistry<TEvent>.Index])
            {
                if (sub.Matches(e))
                    if (!sub.Invoke(this, e, @event))
                        break;
            }
        }

        public static void Subscribe<TComponent, TEvent>(Func<EntityManager, Entity, TComponent, TEvent, bool> onEvent, int priority)
            where TComponent : Component, new()
            where TEvent : GameEvent
        {
            SubscriptionMap[EventRegistry<TEvent>.Index].Add(new CancellableEventSubscription<TComponent, TEvent>(onEvent, priority));
        }

        public static void Subscribe<TComponent, TEvent>(Action<EntityManager, Entity, TComponent, TEvent> onEvent)
            where TComponent : Component, new()
            where TEvent : GameEvent
        {
            SubscriptionMap[EventRegistry<TEvent>.Index].Add(new EventSubscription<TComponent, TEvent>(onEvent, 100));
        }

        public Entity InstantiatePrefab(string key)
        {
#if DEBUG
            if (!_prefabs.ContainsKey(key))
                throw new ArgumentException($"Unknown prefab key {key}");
#endif

            return _prefabs[key](this);
        }

        public void RemoveEntity(Entity entity)
        {
            _removedEntities.Add(entity);
        }

        public void RegisterPrefab(string key, Func<EntityManager, Entity> prefab)
        {
            if (_prefabs.ContainsKey(key))
                throw new ArgumentException($"Duplicate prefab key: {key}");

            _prefabs[key] = prefab;
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
}