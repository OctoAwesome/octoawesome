using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Interface between application and world model.
    /// </summary>
    public sealed class Simulation : IDisposable
    {
        /// <summary>
        /// Gets the resource manager for managing resources.
        /// </summary>
        public IResourceManager ResourceManager { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation is server side or client side.
        /// </summary>
        public bool IsServerSide { get; set; }

        /// <summary>
        /// Gets a list of simulation components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; }

        /// <summary>
        /// Gets the current state of the simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        ///Gets the <see cref="Guid"/> of the currently loaded universe.
        /// </summary>
        public Guid UniverseId { get; }


        private readonly ExtensionService extensionService;

        private readonly List<Entity> entities = new();
        private readonly CountedScopeSemaphore entitiesSemaphore = new();

        private readonly IPool<EntityNotification> entityNotificationPool;

        private IDisposable? simulationSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Simulation"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resources.</param>
        /// <param name="extensionService">The extension service for extending this simulation.</param>
        /// <param name="service">The game service.</param>
        public Simulation(IResourceManager resourceManager, ExtensionService extensionService)
        {
            ResourceManager = resourceManager;

            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();


            this.extensionService = extensionService;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;

            Components = new ComponentList<SimulationComponent>(
                ValidateAddComponent, ValidateRemoveComponent, null, null);

            extensionService.ExecuteExtender(this);
        }

        private void ValidateAddComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to add Components");
        }

        private void ValidateRemoveComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to remove Components");
        }

        /// <summary>
        /// Create a new game(<see cref="IUniverse"/>).
        /// </summary>
        /// <param name="name">The name of the universe.</param>
        /// <param name="rawSeed">The seed used for creating the universe.</param>
        /// <returns>The <see cref="Guid"/> of the created universe.</returns>
        public Guid NewGame(string name, string rawSeed)
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


            Guid guid = ResourceManager.NewUniverse(name, numericSeed);

            Start();

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
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (entity is IUpdateable updateable)
                    updateable.Update(gameTime);
            }


            // Update all Components
            foreach (var component in Components)
                if (component.Enabled)
                    component.Update(gameTime);

            foreach (var planet in ResourceManager.Planets)
                planet.Value.GlobalChunkCache.AfterSimulationUpdate(this);
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

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                Remove(entities[i]);
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


            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var fb in entities)
                {
                    if (fb == entity
                        || (fb.Components.Contains<UniquePositionComponent>()
                            && entity.Components.Contains<UniquePositionComponent>()
                            && fb.Components.TryGet<PositionComponent>(out var existingPosition)
                            && (!entity.Components.TryGet<PositionComponent>(out var newPosComponent)
                                || existingPosition.Position == newPosComponent.Position)))
                    {
                        return;
                    }
                }

            Entity? existing;
            using (var _ = entitiesSemaphore.EnterCountScope())
            {
                existing = entities.FirstOrDefault(x => x.Id == entity.Id);
                if (existing != default && !overwriteExisting)
                    return;
            }

            if (existing != default)
                Remove(existing);

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            extensionService.ExecuteExtender(entity);
            entity.Initialize(ResourceManager);
            entity.Simulation = this;


            using (var _ = entitiesSemaphore.EnterExclusiveScope())
                entities.Add(entity);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Add(entity);
            }
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

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Remove(entity);
            }

            using (var _ = entitiesSemaphore.EnterExclusiveScope())
                entities.Remove(entity);
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

            var _ = entitiesSemaphore.EnterExclusiveScope();
            Remove(entities.First(e => e.Id == entityId));
        }

        /// <summary>
        /// Gets called for receiving new notifications.
        /// </summary>
        /// <param name="value">The new notification.</param>
        public void OnNext(object value)
        {
            //TODO: Ist das hier noch der Richtige Platz?
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
                            RequestEntity(entityNotification);
                            break;
                    }

                    break;
                default:
                    break;
            }
        }


        private void RequestEntity(EntityNotification entityNotification)
        {
            if (!IsServerSide)
                return;

            ;
            //Entity? entity;
            //using (var _ = entitiesSemaphore.EnterCountScope())
            //{
            //    entity = entities.FirstOrDefault(e => e.Id == entityNotification.EntityId);
            //    if (entity == null)
            //        return;
            //}

            //var remoteEntity = new RemoteEntity(entity);
            //remoteEntity.Components.AddIfTypeNotExists(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            //remoteEntity.Components.AddIfNotExists(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 });
            //remoteEntity.Components.AddIfTypeNotExists(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });

            //var newEntityNotification = entityNotificationPool.Rent();
            //newEntityNotification.Entity = remoteEntity;
            //newEntityNotification.Type = EntityNotification.ActionType.Add;

            //networkRelay.OnNext(newEntityNotification);
            //newEntityNotification.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            simulationSubscription?.Dispose();
        }
    }
}
