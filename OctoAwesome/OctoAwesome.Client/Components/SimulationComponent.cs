﻿using OctoAwesome.Runtime;
using System;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Common;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private readonly ExtensionService extensionService;

        private readonly IResourceManager resourceManager;
        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        public Simulation Simulation { get; private set; }

        public IGameService Service { get; }

        public SimulationState State
        {
            get
            {
                if (Simulation != null)
                    return Simulation.State;
                return SimulationState.Undefined;
            }
        }

        public SimulationComponent(OctoGame game, ExtensionService extensionService, IResourceManager resourceManager) : base(game)
        {
            Service = game.Service;
            this.extensionService = extensionService;
            this.resourceManager = resourceManager;
            simulationRelay = new Relay<Notification>();
            simulationSource = resourceManager.UpdateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        public Guid NewGame(string name, string seed)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionService, Service);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionService, Service);
            Simulation.TryLoadGame(guid);
        }

        public override void Update(GameTime gameTime)
        {
            Simulation?.Update(gameTime);
        }

        public void ExitGame()
        {
            if (Simulation == null)
                return;

            Simulation.ExitGame();
            Simulation = null;
        }

        public Player LoginPlayer(string playerName)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Player player = resourceManager.LoadPlayer(playerName);

            player.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            
            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add,  player));

            return player;
        }

        public void LogoutPlayer(Player player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.Remove(player);
        }
    }
}
