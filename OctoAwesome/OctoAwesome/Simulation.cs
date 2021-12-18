﻿using engenious;

using OctoAwesome.Common;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        /// <summary>
        /// Gets the game service.
        /// </summary>
        public IGameService Service { get; }

        /// <summary>
        /// Gets a list of all entities in the simulation.
        /// </summary>
        public IReadOnlyList<Entity> Entities => entities;

        /// <summary>
        /// Gets a list of all functional blocks in the simulation.
        /// </summary>
        public IReadOnlyList<FunctionalBlock> FunctionalBlocks => functionalBlocks;

        private readonly IExtensionResolver extensionResolver;

        private readonly List<Entity> entities = new();
        private readonly List<FunctionalBlock> functionalBlocks = new();
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly Relay<Notification> networkRelay;

        private IDisposable simulationSubscription;
        private IDisposable networkSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="Simulation"/> class.
        /// </summary>
        /// <param name="resourceManager">The resource manager for managing resources.</param>
        /// <param name="extensionResolver">The extension resolver for extending this simulation.</param>
        /// <param name="service">The game service.</param>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver, IGameService service)
        {
            ResourceManager = resourceManager;
            networkRelay = new Relay<Notification>();

            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();


            this.extensionResolver = extensionResolver;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;
            Service = service;

            Components = new ComponentList<SimulationComponent>(
                ValidateAddComponent, ValidateRemoveComponent, null, null);

            extensionResolver.ExtendSimulation(this);

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

            networkSubscription
                = ResourceManager
                .UpdateHub
                .AddSource(networkRelay, DefaultChannels.Network);


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
                if (entity is UpdateableEntity updateableEntity)
                    updateableEntity.Update(gameTime);
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
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: ugly, Dispose Entity's, Reset Extensions
            entities.ToList().ForEach(entity => Remove(entity));
            functionalBlocks.ToList().ForEach(functionalBlock => Remove(functionalBlock));
            //while (entities.Count > 0)
            //    RemoveEntity(Entities.First());

            State = SimulationState.Finished;
            // thread.Join();

            ResourceManager.UnloadUniverse();
            simulationSubscription?.Dispose();
            networkSubscription?.Dispose();
        }

        /// <summary>
        /// Adds an entity to the simulation.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to add.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if simulation is not running or paused. Or if entity is already part of another simulation.
        /// </exception>
        public void Add(Entity entity)
        {
            Debug.Assert(entity is not null, nameof(entity) + " != null");

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if (entity.Simulation != null && entity.Simulation != this)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            if (entities.Contains(entity))
                return;

            extensionResolver.ExtendEntity(entity);
            entity.Initialize(ResourceManager);
            entity.Simulation = this;

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            entities.Add(entity);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Add(entity);
            }
        }

        /// <summary>
        /// Adds a functional block to the simulation.
        /// </summary>
        /// <param name="block">The <see cref="FunctionalBlock"/> to add.</param>
        /// <exception cref="NotSupportedException">
        /// Thrown if simulation is not running or paused. Or if entity is already part of another simulation.
        /// </exception>
        public void Add(FunctionalBlock block)
        {
            Debug.Assert(block is not null, nameof(block) + " != null");

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException($"Adding {nameof(FunctionalBlock)} only allowed in running or paused state");

            if (block.Simulation != null && block.Simulation != this)
                throw new NotSupportedException($"{nameof(FunctionalBlock)} can't be part of more than one simulation");

            if (functionalBlocks.Contains(block))
                return;


            extensionResolver.ExtendEntity(block);
            block.Initialize(ResourceManager);
            block.Simulation = this;

            if (block.Id == Guid.Empty)
                block.Id = Guid.NewGuid();

            functionalBlocks.Add(block);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Add(block);
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

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Removing Entities only allowed in running or paused state");

            ResourceManager.SaveComponentContainer<Entity, IEntityComponent>(entity);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Remove(entity);
            }

            entities.Remove(entity);
            entity.Id = Guid.Empty;
            entity.Simulation = null;

        }

        /// <summary>
        /// Removes a functional block from the simulation.
        /// </summary>
        /// <param name="block">The <see cref="FunctionalBlock"/> to remove.</param>
        public void Remove(FunctionalBlock block)
        {
            Debug.Assert(block is not null, nameof(block) + " != null");

            if (block.Id == Guid.Empty)
                return;

            if (block.Simulation != this)
            {
                if (block.Simulation == null)
                    return;

                throw new NotSupportedException($"{nameof(FunctionalBlock)} can't be removed from a foreign simulation");
            }

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException($"Removing {nameof(FunctionalBlock)} only allowed in running or paused state");


            ResourceManager.SaveComponentContainer<FunctionalBlock, IFunctionalBlockComponent>(block);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Remove(block);
            }

            functionalBlocks.Remove(block);
            block.Id = Guid.Empty;
            block.Simulation = null;

        }

        /// <summary>
        /// Remove an entity by a given id.
        /// </summary>
        /// <param name="entityId">The <see cref="Guid"/> of the entity to remove.</param>
        public void RemoveEntity(Guid entityId)
            => Remove(entities.First(e => e.Id == entityId));

        /// <summary>
        /// Gets called for receiving new notifications.
        /// </summary>
        /// <param name="value">The new notification.</param>
        public void OnNext(Notification value)
        {
            if (entities.Count < 1 && !IsServerSide)
                return;

            switch (value)
            {
                case EntityNotification entityNotification:
                    if (entityNotification.Type == EntityNotification.ActionType.Remove)
                        RemoveEntity(entityNotification.EntityId);
                    else if (entityNotification.Type == EntityNotification.ActionType.Add)
                        Add(entityNotification.Entity);
                    else if (entityNotification.Type == EntityNotification.ActionType.Update)
                        EntityUpdate(entityNotification);
                    else if (entityNotification.Type == EntityNotification.ActionType.Request)
                        RequestEntity(entityNotification);
                    break;
                case FunctionalBlockNotification functionalBlockNotification:
                    if (functionalBlockNotification.Type == FunctionalBlockNotification.ActionType.Add)
                        Add(functionalBlockNotification.Block);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Called when an update notification was received.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        public void OnUpdate(SerializableNotification notification)
        {
            if (!IsServerSide)
                networkRelay.OnNext(notification);
        }

        private void EntityUpdate(EntityNotification notification)
        {
            var entity = entities.FirstOrDefault(e => e.Id == notification.EntityId);
            if (entity == null)
            {
                var entityNotification = entityNotificationPool.Rent();
                entityNotification.EntityId = notification.EntityId;
                entityNotification.Type = EntityNotification.ActionType.Request;
                networkRelay.OnNext(entityNotification);
                entityNotification.Release();
            }
            else
            {
                entity.Push(notification.Notification);
            }
        }

        private void RequestEntity(EntityNotification entityNotification)
        {
            if (!IsServerSide)
                return;

            var entity = entities.FirstOrDefault(e => e.Id == entityNotification.EntityId);

            if (entity == null)
                return;

            var remoteEntity = new RemoteEntity(entity);
            remoteEntity.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            remoteEntity.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remoteEntity.Components.AddComponent(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });

            var newEntityNotification = entityNotificationPool.Rent();
            newEntityNotification.Entity = remoteEntity;
            newEntityNotification.Type = EntityNotification.ActionType.Add;

            networkRelay.OnNext(newEntityNotification);
            newEntityNotification.Release();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            networkRelay.Dispose();
        }
    }
}
