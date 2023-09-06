using engenious;

using NLog;

using NonSucking.Framework.Extension.Collections;

using OctoAwesome.Caching;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

namespace OctoAwesome
{
    /// <summary>
    /// Interface between application and world model.
    /// </summary>
    public sealed class Simulation : ComponentContainer<SimulationComponent>, IDisposable
    {
        /// <summary>
        /// Gets the resource manager for managing resources.
        /// </summary>
        public IResourceManager ResourceManager { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation is server side.
        /// </summary>
        public bool IsServerSide { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation is client side.
        /// </summary>
        public bool IsClientSide => !IsServerSide;

        /// <summary>
        /// Gets a list of simulation components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; }

        public ComponentList<IComponent> GlobalComponentList { get; }

        /// <summary>
        /// Gets the current state of the simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        /// Gets the <see cref="Guid"/> of the currently loaded universe.
        /// </summary>
        public Guid UniverseId { get; }

        /// <summary>
        /// Gets the <see cref="FooBbqSimulationComponent"/> of the current universe for sending components via Network
        /// </summary>
        public NetworkingSimulationComponent FooBbqSimulationComponent { get; }

        private readonly Logger logger;
        private readonly ExtensionService extensionService;

        private readonly EnumerationModifiableConcurrentList<Entity> entities = new();
        private readonly CountedScopeSemaphore entitiesSemaphore = new();

        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly ComponentChangedNotificationHandler componentChangedHandler;
        private IDisposable? simulationSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Simulation"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resources.</param>
        /// <param name="extensionService">The extension service for extending this simulation.</param>
        public Simulation(IResourceManager resourceManager, ExtensionService extensionService)
        {
            ResourceManager = resourceManager;
            var typeContainer = TypeContainer.Get<ITypeContainer>();
            entityNotificationPool = typeContainer.Get<IPool<EntityNotification>>();
            componentChangedHandler = typeContainer.Get<ComponentChangedNotificationHandler>();
            componentChangedHandler.AssociatedSimulation = this;
            FooBbqSimulationComponent = new NetworkingSimulationComponent(
                typeContainer.Get<IUpdateHub>(),
                entityNotificationPool,
                typeContainer.Get<IPool<PropertyChangedNotification>>());
            this.extensionService = extensionService;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;
            logger = NLog.LogManager.GetCurrentClassLogger();
            GlobalComponentList = new();

            Components = new ComponentList<SimulationComponent>(
                ValidateAddComponent, ValidateRemoveComponent, null, null, this);

            extensionService.ExecuteExtender(this);
        }

        private void ValidateAddComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to add Components");
            GlobalComponentList.Add(component);
        }

        private void ValidateRemoveComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to remove Components");
            GlobalComponentList.Remove(component);
        }

        /// <summary>
        /// Create a new game(<see cref="IUniverse"/>).
        /// </summary>
        /// <param name="name">The name of the universe.</param>
        /// <param name="rawSeed">The seed used for creating the universe.</param>
        /// <returns>The <see cref="Guid"/> of the created universe.</returns>
        public static Guid NewGame(IResourceManager resourceManager, string  name, string rawSeed)
        {
            int numericSeed;

            if (string.IsNullOrWhiteSpace(rawSeed))
            {
                var rand = new Random();
                numericSeed = rand.Next(int.MaxValue);
            }
            else if (int.TryParse(rawSeed, out var seed))
            {
                numericSeed = seed;
            }
            else
            {
                numericSeed = rawSeed.GetHashCode();
            }


            Guid guid = resourceManager.NewUniverse(name, numericSeed);

            return guid;
        }

        /// <summary>
        /// Loads a game(<see cref="IUniverse"/>).
        /// </summary>
        /// <param name="guid">The <see cref="Guid"/> of the universe to load.</param>
        public bool TryLoadGame(Guid guid)
        {
            if (!ResourceManager.TryLoadUniverse(guid))
                return false;

            Start();
            return true;
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new Exception();

            simulationSubscription
                = ResourceManager
                .UpdateHub
                .ListenOn(DefaultChannels.Simulation)
                .Subscribe(OnNext);

            State = SimulationState.Running;
        }

        /// <summary>
        /// Call to update and advance the simulation.
        /// </summary>
        /// <param name="gameTime">The game time to advance the simulation by.</param>
        public void Update(GameTime gameTime)
        {
            if (State != SimulationState.Running)
                return;

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.BeforeSimulationUpdate(this);

            //Update all Entities
            if (entities.Count > 0)
                foreach (Entity entity in entities)
                    if (entity is IUpdateable updateable)
                        updateable.Update(gameTime);

            // Update all Components
            foreach (var component in Components)
                if (component.Enabled)
                    component.Update(gameTime);

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.AfterSimulationUpdate(this);

            FooBbqSimulationComponent.Update(gameTime);
        }

        /// <summary>
        /// Exits the current simulation.
        /// </summary>
        /// <remarks>This does not exit the application.</remarks>
        public void ExitGame()
        {
            if (State is not SimulationState.Running and not SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: unschön, Dispose Entity's, Reset Extensions

            foreach (var entity in entities)
            {
                Remove(entity);
            }

            State = SimulationState.Finished;

            ResourceManager.UnloadUniverse();
            simulationSubscription?.Dispose();
        }

        /// <summary>
        /// Adds an entity to the simulation.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to add.</param>
        /// <param name="overwriteExisting">Indicates if an existing entity should be overwritten by the new one</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if simulation is not running or paused. Or if entity is already part of another simulation.
        /// </exception>
        public void Add(Entity entity, bool overwriteExisting)
        {
            Debug.Assert(entity is not null, nameof(entity) + " != null");

            if (State is not (SimulationState.Running or SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if (entity.Simulation is not null && entity.Simulation != this)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            var hasPosComponent = entity.Components.TryGet<PositionComponent>(out var newPosComponent);
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var fb in entities)
                {
                    if (fb == entity
                        || (fb.Components.Contains<UniquePositionComponent>()
                            && entity.Components.Contains<UniquePositionComponent>()
                            && fb.Components.TryGet<PositionComponent>(out var existingPosition)
                            && (!hasPosComponent || existingPosition.Position == newPosComponent!.Position)))
                    {
                        SendToClientIfRequired(entity, true, EntityNotification.ActionType.Add);
                        return;
                    }
                }

            Entity? existing;
            using (var _ = entitiesSemaphore.EnterCountScope())
            {
                existing = entities.FirstOrDefault(x => x.Id == entity.Id);
                if (existing != default && !overwriteExisting)
                {
                    SendToClientIfRequired(entity, true, EntityNotification.ActionType.Add);
                    return;
                }
            }

            if (existing != default)
                Remove(existing);

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entity.ResourceManager = ResourceManager;
            entity.Simulation = this;
            extensionService.ExecuteExtender(entity);
            entity.Initialize();

            if (!hasPosComponent)
                hasPosComponent = entity.Components.TryGet<PositionComponent>(out newPosComponent);

            //using (var _ = entitiesSemaphore.EnterExclusiveScope())
            entities.Add(entity);
            
            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Add(entity);
            }

            if (IsServerSide)
            {
                entity.Components.AddIfTypeNotExists(new ServerManagedComponent { OnServer = true });
            }

            if (hasPosComponent)
            {
                var gcc = newPosComponent.Planet.GlobalChunkCache;
                gcc.CacheService.AddOrUpdate(entity.Id, newPosComponent);
                gcc.CacheService.AddOrUpdate(entity.Id, entity);
            }

            foreach (var comp in entity.Components)
            {
                GlobalComponentList.AddIfNotExists(comp);
            }

            SendToClientIfRequired(entity, false, EntityNotification.ActionType.Add);
        }

        private void SendToClientIfRequired(Entity entity, bool existsAlready, EntityNotification.ActionType type)
        {
            if ((entity is RemoteEntity && IsClientSide)
                    || (!existsAlready && entity is Player && IsServerSide))
                return;

            logger.Debug($"Send {entity.GetType().Name} with id {entity.Id} to network");
            if (entity is not RemoteEntity)
                entity = new RemoteEntity(entity);

            var remoteNotification = new EntityNotification
            {
                Entity = entity,
                Type = type
            };

            ResourceManager.UpdateHub.PushNetwork(remoteNotification, DefaultChannels.Simulation);
        }

        /// <summary>
        /// Removes an entity from the simulation.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to remove.</param>
        public void Remove(Entity entity)
        {
            Debug.Assert(entity is not null, nameof(entity) + " != null");

            if (entity.Id == Guid.Empty)
                return;

            if (entity.Simulation != this)
            {
                if (entity.Simulation == null)
                    return;

                throw new NotSupportedException("Entity can't be removed from a foreign simulation");
            }

            if (State is not (SimulationState.Running or SimulationState.Paused))
                throw new NotSupportedException("Removing Entities only allowed in running or paused state");

            ResourceManager.SaveComponentContainer<Entity, IEntityComponent>(entity);


            foreach (var comp in entity.Components)
            {
                GlobalComponentList.Remove(comp);
            }

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Remove(entity);
            }

            //using (var _ = entitiesSemaphore.EnterExclusiveScope())
            entities.Remove(entity);
            SendToClientIfRequired(entity, true, EntityNotification.ActionType.Remove);
            entity.Id = Guid.Empty;
            entity.Simulation = null;
        }

        /// <summary>
        /// Search and get entities by a specified type from this simulation
        /// </summary>
        /// <typeparam name="T">The type to search for</typeparam>
        /// <returns>A collection of <typeparamref name="T"/> of the entities that are part of this simulation</returns>
        public IReadOnlyCollection<T> GetEntitiesOfType<T>()
        {
            var ret = new List<T>();
            using var _ = entitiesSemaphore.EnterCountScope();
            foreach (var item in entities)
            {
                if (item is T t)
                    ret.Add(t);
            }
            return ret;
        }

        /// <summary>
        /// Search and get <see cref="ComponentContainer"/> by a specified <see cref="Component"/> type from this simulation
        /// </summary>
        /// <typeparam name="T">The component type to search for</typeparam>
        /// <returns>A collection of <see cref="ComponentContainer"/> that are part of this simulation</returns>
        public IReadOnlyCollection<ComponentContainer> GetByComponentType<T>()
        {
            var ret = new List<ComponentContainer>();
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item.Components.Contains<T>())
                        ret.Add(item);
                }

            return ret;
        }


        /// <summary>
        /// Search and get <see cref="ComponentContainer"/> by both specified <see cref="Component"/> type from this simulation
        /// </summary>
        /// <typeparam name="T1">The component type to search for</typeparam>
        /// <typeparam name="T2">The component type to search for</typeparam>
        /// <returns>A collection of <see cref="ComponentContainer"/> that are part of this simulation and matches the component types</returns>
        public IReadOnlyCollection<ComponentContainer> GetByComponentTypes<T1, T2>()
        {
            var ret = new List<ComponentContainer>();
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item.Components.Contains<T1>() && item.Components.Contains<T2>())
                        ret.Add(item);
                }

            return ret;
        }

        /// <summary>
        /// Search and get <see cref="ComponentContainer"/> by a specified id
        /// </summary>
        /// <typeparam name="T">The <see cref="ComponentContainer"/> to search for</typeparam>
        /// <returns><see cref="ComponentContainer"/> that has been found or <see langword="default"/></returns>
        public T? GetById<T>(Guid id) where T : ComponentContainer
        {
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item is T t && t.Id == id)
                        return t;
                }

            return default;
        }

        /// <summary>
        /// Try search and get <see cref="ComponentContainer"/> by a specified id
        /// </summary>
        /// <typeparam name="T">The <see cref="ComponentContainer"/> to search for</typeparam>
        /// <param name="id">The id to search by.</param>
        /// <param name="componentContainer"><see cref="ComponentContainer"/> that has been found or <see langword="default"/></param>
        /// <returns><see langword="true"/> if a container was found, otherwise <see langword="false"/></returns>
        public bool TryGetById<T>(Guid id, [MaybeNullWhen(false)] out T componentContainer) where T : ComponentContainer
        {
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item is not T t || t.Id != id)
                        continue;
                    componentContainer = t;
                    return true;
                }

            componentContainer = default;
            return false;
        }

        /// <summary>
        /// Remove an entity by a given id.
        /// </summary>
        /// <param name="entityId">The <see cref="Guid"/> of the entity to remove.</param>
        public void RemoveEntity(Guid entityId)
        {
            Remove(entities.First(e => e.Id == entityId));
        }

        /// <summary>
        /// Gets called for receiving new notifications.
        /// </summary>
        /// <param name="value">The new notification.</param>
        public void OnNext(object value)
        {
            switch (value)
            {
                case EntityNotification entityNotification:
                    switch (entityNotification.Type)
                    {
                        case EntityNotification.ActionType.Remove:
                            RemoveEntity(entityNotification.EntityId);
                            break;
                        case EntityNotification.ActionType.Add:
                            Add(entityNotification.Entity, entityNotification.OverwriteExisting);
                            break;
                        case EntityNotification.ActionType.Request:
                            RequestEntity(entityNotification.EntityId);
                            break;
                        case EntityNotification.ActionType.Update:
                            EntityUpdate(entityNotification);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
        private void EntityUpdate(EntityNotification notification)
        {
            var instance = entities.FirstOrDefault(x => x.Id == notification.EntityId);
            if (instance is null)
                return;

            componentChangedHandler.Execute(instance, notification);
        }

        private void RequestEntity(Guid entityId)
        {
            if (!IsServerSide)
                return;

            Entity? entity;
            using (var _ = entitiesSemaphore.EnterCountScope())
            {
                entity = entities.FirstOrDefault(e => e.Id == entityId);
                if (entity == null)
                    return;
            }

            var remoteEntity = new RemoteEntity(entity);
            remoteEntity.Components.AddIfTypeNotExists(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            remoteEntity.Components.AddIfNotExists(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 });
            remoteEntity.Components.AddIfTypeNotExists(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });

            var newEntityNotification = entityNotificationPool.Rent();
            newEntityNotification.Entity = remoteEntity;
            newEntityNotification.Type = EntityNotification.ActionType.Add;

            ResourceManager.UpdateHub.PushNetwork(newEntityNotification, DefaultChannels.Simulation);
            newEntityNotification.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            simulationSubscription?.Dispose();
        }
    }
}
