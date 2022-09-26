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
        private Simulation? simulation;

        public Simulation Simulation
        {
            get => NullabilityHelper.NotNullAssert(simulation, $"{nameof(Simulation)} was not initialized!");
        }

        public IGameService Service { get; }

        public SimulationState State => simulation?.State ?? SimulationState.Undefined;

        public SimulationComponent(OctoGame game, ExtensionService extensionService, IResourceManager resourceManager) : base(game)
        {
            Service = game.Service;
            this.extensionService = extensionService;
            this.resourceManager = resourceManager;
            simulationRelay = new Relay<Notification>();
            simulationSource = resourceManager.UpdateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
            Enabled = false;
        }

        private void ExitSimulation()
        {
            Enabled = false;
            simulation?.ExitGame();
            simulation = null;
        }

        public Guid NewGame(string name, string seed)
        {
            ExitSimulation();

            simulation = new Simulation(resourceManager, extensionService, Service);
            var newGame = Simulation.NewGame(name, seed);
            Enabled = true;
            return newGame;
        }

        public void LoadGame(Guid guid)
        {
            ExitSimulation();

            simulation = new Simulation(resourceManager, extensionService, Service);
            if (Simulation.TryLoadGame(guid))
                Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            Simulation.Update(gameTime);
        }

        public void ExitGame()
        {
            ExitSimulation();
        }

        public Player LoginPlayer(string playerName)
        {
            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Player player = resourceManager.LoadPlayer(playerName);

            player.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);

            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add, player) { OverwriteExisting = true });

            return player;
        }

        public void LogoutPlayer(Player player)
        {
            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.Remove(player);
        }
    }
}
