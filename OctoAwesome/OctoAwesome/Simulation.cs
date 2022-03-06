﻿using engenious;

using OctoAwesome.Common;
using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation : IDisposable
    {
        public IResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// Checks wether the Simulation is running on a server
        /// </summary>
        public bool IsServerSide { get; set; }

        /// <summary>
        /// List of all Simulation Components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; private set; }

        /// <summary>
        /// Der aktuelle Status der Simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        /// Die Guid des aktuell geladenen Universums.
        /// </summary>
        public Guid UniverseId { get; private set; }

        /// <summary>
        /// Dienste des Spiels.
        /// </summary>
        public IGameService Service { get; }

        ///// <summary>
        ///// List of all Entities.
        ///// </summary>
        //public IReadOnlyList<Entity> Entities => entities;
        ///// <summary>
        ///// List of all FunctionalBlocks.
        ///// </summary>
        //public IReadOnlyList<FunctionalBlock> FunctionalBlocks => functionalBlocks;

        private readonly ExtensionService extensionService;

        private readonly List<Entity> entities = new();
        private readonly CountedScopeSemaphore entitiesSemaphore = new();


        private readonly List<FunctionalBlock> functionalBlocks = new();
        private readonly CountedScopeSemaphore functionalBlocksSemaphore = new();

        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly Relay<Notification> networkRelay;
        private readonly Relay<Notification> uiRelay;

        private IDisposable simulationSubscription;
        private IDisposable networkSubscription;
        private IDisposable uiSubscription;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, ExtensionService extensionService, IGameService service)
        {
            ResourceManager = resourceManager;
            networkRelay = new Relay<Notification>();
            uiRelay = new Relay<Notification>();

            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();


            this.extensionService = extensionService;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;
            Service = service;

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
        /// Erzeugt ein neues Spiel (= Universum)
        /// </summary>
        /// <param name="name">Name des Universums.</param>
        /// <param name="rawSeed">Seed für den Weltgenerator.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
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
        /// Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
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

            uiSubscription
                = ResourceManager
                .UpdateHub
                .AddSource(uiRelay, DefaultChannels.UI);


            State = SimulationState.Running;
        }

        /// <summary>
        /// Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
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
        /// Beendet das aktuelle Spiel (nicht die Applikation)
        /// </summary>
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

            for (int i = functionalBlocks.Count - 1; i >= 0; i--)
            {
                Remove(functionalBlocks[i]);
            }

            //while (entites.Count > 0)
            //    RemoveEntity(Entities.First());

            State = SimulationState.Finished;
            // thread.Join();

            ResourceManager.UnloadUniverse();
            simulationSubscription?.Dispose();
            networkSubscription?.Dispose();
            uiSubscription?.Dispose();
        }

        /// <summary>
        /// Fügt eine Entity der Simulation hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void Add(Entity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (State is not (SimulationState.Running or SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if (entity.Simulation is not null && entity.Simulation != this)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            using (var _ = entitiesSemaphore.EnterCountScope())
                if (entities.Contains(entity))
                    return;


            extensionService.ExecuteExtender(entity);
            entity.Initialize(ResourceManager);
            entity.Simulation = this;

            if (entity.Id == Guid.Empty)
                entity.Id = Guid.NewGuid();

            using (var _ = entitiesSemaphore.EnterExclusivScope())
                entities.Add(entity);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<Entity> holdComponent)
                    holdComponent.Add(entity);
            }
        }

        public void Add(FunctionalBlock block)
        {
            if (block is null)
                throw new ArgumentNullException(nameof(block));

            if (State is not (SimulationState.Running or SimulationState.Paused))
                throw new NotSupportedException($"Adding {nameof(FunctionalBlock)} only allowed in running or paused state");

            if (block.Simulation is not null && block.Simulation != this)
                throw new NotSupportedException($"{nameof(FunctionalBlock)} can't be part of more than one simulation");


            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                foreach (var fb in functionalBlocks)
                {
                    if (fb == block
                        || (fb.Components.TryGetComponent<PositionComponent>(out var existing)
                            && (!block.Components.TryGetComponent<PositionComponent>(out PositionComponent newPosComponent)
                                || existing.Position == newPosComponent.Position)))
                    {
                        return;
                    }
                }

            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                if (functionalBlocks.Contains(block))
                    return;



            extensionService.ExecuteExtender(block);
            block.Initialize(ResourceManager);
            block.Simulation = this;

            if (block.Id == Guid.Empty)
                block.Id = Guid.NewGuid();

            using (var _ = functionalBlocksSemaphore.EnterExclusivScope())
                functionalBlocks.Add(block);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Add(block);
            }
        }

        /// <summary>
        /// Entfernt eine Entity aus der Simulation
        /// </summary>
        /// <param name="entity">Entity die entfert werden soll</param>
        public void Remove(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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

            using (var _ = entitiesSemaphore.EnterExclusivScope())
                entities.Remove(entity);
            entity.Id = Guid.Empty;
            entity.Simulation = null;

        }

        public void Remove(FunctionalBlock block)
        {
            if (block == null)
                throw new ArgumentNullException(nameof(block));

            if (block.Id == Guid.Empty)
                return;

            if (block.Simulation != this)
            {
                if (block.Simulation == null)
                    return;

                throw new NotSupportedException($"{nameof(FunctionalBlock)} can't be removed from a foreign simulation");
            }

            if (State is not (SimulationState.Running or SimulationState.Paused))
                throw new NotSupportedException($"Removing {nameof(FunctionalBlock)} only allowed in running or paused state");


            ResourceManager.SaveComponentContainer<FunctionalBlock, IFunctionalBlockComponent>(block);

            foreach (var component in Components)
            {
                if (component is IHoldComponent<FunctionalBlock> holdComponent)
                    holdComponent.Remove(block);
            }

            using (var _ = functionalBlocksSemaphore.EnterExclusivScope())
                functionalBlocks.Remove(block);
            block.Id = Guid.Empty;
            block.Simulation = null;

        }

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

        public IReadOnlyCollection<ComponentContainer> GetByComponentType<T>()
        {
            var ret = new List<ComponentContainer>();
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item.Components.ContainsComponent<T>())
                        ret.Add(item);
                }
            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                foreach (var item in functionalBlocks)
                {
                    if (item.Components.ContainsComponent<T>())
                        ret.Add(item);
                }
            return ret;
        }
        public IReadOnlyCollection<ComponentContainer> GetByComponentTypes<T1, T2>()
        {
            var ret = new List<ComponentContainer>();
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item.Components.ContainsComponent<T1>() && item.Components.ContainsComponent<T2>())
                        ret.Add(item);
                }
            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                foreach (var item in functionalBlocks)
                {
                    if (item.Components.ContainsComponent<T1>() && item.Components.ContainsComponent<T2>())
                        ret.Add(item);
                }
            return ret;
        }

        public T GetById<T>(Guid id) where T : ComponentContainer
        {
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (item is T t && t.Id == id)
                        return t;
                }

            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                foreach (var item in functionalBlocks)
                {
                    if (item is T t && t.Id == id)
                        return t;
                }

            return default;
        }
        public bool TryGetById<T>(Guid id, out T componentContainer) where T : ComponentContainer
        {
            using (var _ = entitiesSemaphore.EnterCountScope())
                foreach (var item in entities)
                {
                    if (!(item is T t) || t.Id != id)
                        continue;
                    componentContainer = t;
                    return true;
                }

            using (var _ = functionalBlocksSemaphore.EnterCountScope())
                foreach (var item in functionalBlocks)
                {
                    if (!(item is T t) || t.Id != id)
                        continue;
                    componentContainer = t;
                    return true;
                }
            componentContainer = default;
            return false;
        }

        public void RemoveEntity(Guid entityId)
        {

            var _ = entitiesSemaphore.EnterExclusivScope();
            Remove(entities.First(e => e.Id == entityId));
        }

        public void OnNext(Notification value)
        {
            if (entities.Count < 0 && !IsServerSide)
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

                    uiRelay.OnNext(value);
                    break;
                case FunctionalBlockNotification functionalBlockNotification:
                    if (functionalBlockNotification.Type == FunctionalBlockNotification.ActionType.Add)
                        Add(functionalBlockNotification.Block);

                    uiRelay.OnNext(value);
                    break;
                default:
                    break;
            }
        }


        public void OnUpdate(SerializableNotification notification)
        {
            if (!IsServerSide)
                networkRelay.OnNext(notification);
        }

        private void EntityUpdate(EntityNotification notification)
        {
            Entity? entity;
            using (var _ = entitiesSemaphore.EnterCountScope())
                entity = entities.FirstOrDefault(e => e.Id == notification.EntityId);
            if (entity == null)
            {
                var entityNotification = entityNotificationPool.Get();
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

            Entity? entity;
            using (var _ = entitiesSemaphore.EnterCountScope())
            {
                entity = entities.FirstOrDefault(e => e.Id == entityNotification.EntityId);
                if (entity == null)
                    return;
            }

            var remoteEntity = new RemoteEntity(entity);
            remoteEntity.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            remoteEntity.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remoteEntity.Components.AddComponent(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });

            var newEntityNotification = entityNotificationPool.Get();
            newEntityNotification.Entity = remoteEntity;
            newEntityNotification.Type = EntityNotification.ActionType.Add;

            networkRelay.OnNext(newEntityNotification);
            newEntityNotification.Release();
        }

        public void Dispose()
        {
            uiRelay.Dispose();
            networkRelay.Dispose();
        }
    }
}
