﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using engenious;
using OctoAwesome.Entities;
using OctoAwesome.Common;

namespace OctoAwesome
{
    /// <summary>
    /// Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation
    {
        // private List<ActorHost> actorHosts = new List<ActorHost>();
        // private Stopwatch watch = new Stopwatch();
        // private Thread thread;
        /// <summary>
        /// Manager for Resources
        /// </summary>
        public IResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// List of all Simulation Components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; private set; }

        /// <summary>
        /// Der aktuelle Status der Simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        /// List of all Entities.
        /// </summary>
        public List<Entity> Entities => entities.ToList();

        private int nextId = 1;

        private readonly IExtensionResolver extensionResolver;

        private HashSet<Entity> entities = new HashSet<Entity>();
        private IGameService service;

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver, IGameService service)
        {
            ResourceManager = resourceManager;
            this.service = service;
            this.extensionResolver = extensionResolver;
            State = SimulationState.Ready;

            Components = new ComponentList<SimulationComponent>(ValidateAddComponent, ValidateRemoveComponent);

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
        /// Erzeugt ein neues Spiel (= Universum)
        /// </summary>
        /// <param name="name">Name des Universums.</param>
        /// <param name="seed">Seed für den Weltgenerator.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewGame(string name, int? seed = null)
        {
            if (seed == null)
            {
                Random rand = new Random();
                seed = rand.Next(int.MaxValue);
            }

            Guid guid = ResourceManager.NewUniverse(name, seed.Value);

            Start();

            return guid;
        }

        /// <summary>
        /// Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
        public void LoadGame(Guid guid)
        {
            ResourceManager.LoadUniverse(guid);
            Start();
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new Exception();

            State = SimulationState.Running;
        }

        /// <summary>
        /// Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
            {
                ResourceManager.GlobalChunkCache.BeforeSimulationUpdate(this);

                // TODO: Seperate updatealbe entities.
                //Update all Entities
                foreach (var entity in Entities)
                {
                    if(entity.NeedUpdate)
                        entity.Update(gameTime, service);
                    entity.Components.Update(gameTime, service);
                }

                // Update all Components
                foreach (var component in Components.Where(c => c.Enabled))
                    component.Update(gameTime, service);

                ResourceManager.GlobalChunkCache.AfterSimulationUpdate(this);
            }
        }
        /// <summary>
        /// Beendet das aktuelle Spiel (nicht die Applikation)
        /// </summary>
        public void ExitGame()
        {
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: unschön
            Entities.ForEach(entity => RemoveEntity(entity));

            State = SimulationState.Finished;
            // thread.Join();

            ResourceManager.UnloadUniverse();
        }

        /// <summary>
        /// Fügt eine Entity der Simulation hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void AddEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if(entities.Contains(entity))
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            entity.Initialize(service);
            extensionResolver.ExtendEntity(entity);
            entity.SetPosition(entity.Position, entity.Azimuth);
            entity.Id = nextId++;
            entities.Add(entity);

            // TODO: wegschmeisen
            foreach (var component in Components)
                component.Register(entity);
        }

        /// <summary>
        /// Entfernt eine Entity aus der Simulation
        /// </summary>
        /// <param name="entity">Entity die entfert werden soll</param>
        public void RemoveEntity(Entity entity)
        {
            if (entity.Id == 0)
                return;

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            foreach (var component in Components)
                component.Remove(entity);

            entities.Remove(entity);
            entity.Id = 0;

            ResourceManager.SaveEntity(entity);
        }
    }
}
