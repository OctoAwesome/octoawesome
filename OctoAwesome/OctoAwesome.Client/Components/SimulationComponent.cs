using System;
using engenious;
using OctoAwesome.EntityComponents;
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


        public SimulationState State => simulation?.State ?? SimulationState.Undefined;

        public SimulationComponent(OctoGame game, ExtensionService extensionService, IResourceManager resourceManager) : base(game)
        {
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
            var newGame = Simulation.NewGame(resourceManager, name, seed);
            return newGame;
        }

        public void LoadGame(Guid guid)
        {
            ExitSimulation();
            
            simulation = new Simulation(resourceManager, extensionService);
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
