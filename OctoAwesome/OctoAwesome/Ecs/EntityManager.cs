using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using engenious;

namespace OctoAwesome.Ecs
{
    public class EntityManager
    {
        private readonly List<Entity> _addedEntities = new List<Entity>();
        private readonly List<Entity> _changedEntities = new List<Entity>();

        private readonly List<Entity> _entities = new List<Entity>();

        private readonly List<Action> _pendingActions = new List<Action>(512);

        private readonly Dictionary<string, Func<EntityManager, Entity>> _prefabs = new Dictionary<string, Func<EntityManager, Entity>>();
        private readonly List<Entity> _removedEntities = new List<Entity>();
        internal static readonly List<Func<EntityManager, BaseSystem>> SystemConstructorsnUpdateOrder = new List<Func<EntityManager, BaseSystem>>();
        internal readonly List<BaseSystem> Systems;
        internal readonly List<List<BaseSystem>> UpdateGroups;

        public static readonly Dictionary<string, Action<Entity, BinaryReader>> Deserializers;

        public static readonly int ComponentCount;
        private int _lastIndex = 0;

        private static readonly List<List<EventSubscription>> SubscriptionMap;
        
        static EntityManager()
        {
            var baseEventType = typeof(GameEvent);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var nonAbstractTypes = assemblies.SelectMany(a => a.GetTypes()).Where(t => !t.IsAbstract).ToList();

            var eventTypes = nonAbstractTypes
                .Where(t => baseEventType.IsAssignableFrom(t))
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
            var componentTypes = nonAbstractTypes
                .Where(t => baseComponentType.IsAssignableFrom(t))
                .ToList();

            registryType = typeof(ComponentRegistry<>);
            
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

            var baseSystemType = typeof(BaseSystem);
            
            var systemTypes = nonAbstractTypes
               .Where(t => baseSystemType.IsAssignableFrom(t))
               .ToList();

            var validSystems = new List<Tuple<Type, SystemConfigurationAttribute>>();

            foreach (var st in systemTypes)
            {
                var config = st.GetCustomAttributes(typeof(SystemConfigurationAttribute)).FirstOrDefault();
                if(config == null)
                    continue;

                validSystems.Add(Tuple.Create(st, (SystemConfigurationAttribute)config));
            }

            var systemMap = new Dictionary<string, Tuple<Type, SystemConfigurationAttribute>>();

            foreach (var vs in validSystems)
            {
                Action<string[]> validateNames = (arr) => {
                    if (arr == null || arr.Length == 0)
                        return;

                    foreach (var item in arr)
                    {
                        if(validSystems.Any(s => s.Item1.Name == item))
                            break;

                        throw new ArgumentException($"System named `{item}` not found (Referenced by {vs.Item1.FullName})");
                    }
                };

                validateNames(vs.Item2.After);
                validateNames(vs.Item2.Before);
                validateNames(vs.Item2.ConcurrentlyWith);
                validateNames(vs.Item2.Replaces);
            }

            foreach (var vs in validSystems)
            {
                var replacement = vs.Item2.Replaces;
                if(replacement == null || replacement.Length == 0)
                    continue;

                foreach (var item in replacement)
                    systemMap[item] = vs;
            }

            for (int i = 0; i < validSystems.Count; i++)
            {
                if (systemMap.ContainsKey(validSystems[i].Item1.Name))
                {
                    validSystems.RemoveAt(i--);
                }
                else
                {
                    systemMap[validSystems[i].Item1.Name] = validSystems[i];
                }
            }

            var systems = validSystems.Where(i => (i.Item2.After == null || i.Item2.After.Length == 0) && (i.Item2.Before == null || i.Item2.Before.Length == 0)).ToList();

            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < validSystems.Count; i++)
                {
                    var item = validSystems[i];
                    if (systems.Contains(item))
                        continue;

                    Func<string[], bool> allPresent = (arr) => {
                        return arr.All(s => systems.Contains(systemMap[s]));
                    };

                    var hasDependencies = item.Item2.After != null && item.Item2.After.Length > 0;
                    var affects = item.Item2.Before != null && item.Item2.Before.Length > 0;

                    if (hasDependencies && affects)
                    {
                        if (allPresent(item.Item2.After) && allPresent(item.Item2.Before))
                        {
                            var latest = item.Item2.After.Select(n => systems.IndexOf(systemMap[n])).Max();
                            var least = item.Item2.Before.Select(n => systems.IndexOf(systemMap[n])).Min();

                            int index = -1;

                            if (least > latest)
                                index = least;
                            else if (least == latest)
                            {
                                index = least - 1;
                            }
                            else
                            {
                                continue;
                            }

                            if (index == systems.Count - 1)
                                systems.Add(item);
                            else
                                systems.Insert(index, item);
                        }
                    }
                    else if (hasDependencies)
                    {
                        if (allPresent(item.Item2.After))
                        {
                            var index = item.Item2.After.Select(n => systems.IndexOf(systemMap[n])).Max();
                            if (index == systems.Count - 1)
                                systems.Add(item);
                            else
                                systems.Insert(index + 1, item);
                            validSystems.RemoveAt(i--);
                        }
                    }
                    else if (affects)
                    {
                        if (allPresent(item.Item2.Before))
                        {
                            var index = item.Item2.Before.Select(n => systems.IndexOf(systemMap[n])).Min();
                            systems.Insert(index, item);
                            validSystems.RemoveAt(i--);
                        }
                    }
                }
            }


            // TODO: reorder systems to resolve systems with before / after dependencies

            var types = systems.Select(i => i.Item1).ToList();
            var arg = Expression.Parameter(typeof(EntityManager));
            SystemConstructorsnUpdateOrder = types
                .Select(t => t.GetConstructor(new [] {typeof(EntityManager) }))
                .Select(ci => Expression.New(ci, arg))
                .Select(e => Expression.Lambda<Func<EntityManager, BaseSystem>>(e, false, arg).Compile())
                .ToList();

            foreach (var map in SubscriptionMap)
                map.Sort();
        }

        public EntityManager()
        {
            Systems = SystemConstructorsnUpdateOrder.Select(c => c(this)).ToList();
            UpdateGroups = Systems.Select(s => new List<BaseSystem> {s}).ToList();
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
            for (int i = 0; i < UpdateGroups.Count; i++)
            {
                for (int j = 0; j < UpdateGroups[i].Count; j++)
                {
                    //Parallel.ForEach(UpdateGroups[i], s => s.Tick());
                    UpdateGroups[i][j].Tick();
                }
            }

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
}