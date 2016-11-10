using OctoAwesome.Runtime;
using System;
using engenious;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private readonly IExtensionResolver extensionResolver;

        private Simulation Simulation { get; set; }

        public SimulationComponent(Game game, IExtensionResolver extensionResolver) : base(game)
        {
            this.extensionResolver = extensionResolver;
        }

        public Guid NewGame(string name, int? seed = null)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(ResourceManager.Instance, extensionResolver);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(ResourceManager.Instance, extensionResolver);
            Simulation.LoadGame(guid);
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

        public ActorHost InsertPlayer(Player player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.AddEntity(player); // InsertPlayer(player);
            return null;
        }

        public void RemovePlayer(ActorHost host)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            // Simulation.RemovePlayer(host);
            Simulation.RemoveEntity(host.Player);
        }
    }
}
