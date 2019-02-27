using OctoAwesome.Runtime;
using System;
using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private readonly IExtensionResolver extensionResolver;

        private readonly IResourceManager resourceManager;

        public Simulation Simulation { get; private set; }

        public SimulationState State
        {
            get
            {
                if (Simulation != null)
                    return Simulation.State;
                return SimulationState.Undefined;
            }
        }

        public SimulationComponent(Game game, IExtensionResolver extensionResolver, IResourceManager resourceManager) : base(game)
        {
            this.extensionResolver = extensionResolver;
            this.resourceManager = resourceManager;
        }

        public Guid NewGame(string name, int? seed = null)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionResolver);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionResolver);
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

        public Player LoginPlayer(Guid id)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            //TODO: [Network] Anstelle von ID einen einstellbaren Playernamen implementieren
            Player player = resourceManager.LoadPlayer(id.ToString());
            player.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Simulation.AddEntity(player);

            //TODO: Only Debugging
            //var remotePlayer = new RemoteEntity();

            //remotePlayer.Components.AddComponent(new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 75), new Vector3(0, 0, 0)) });
            //remotePlayer.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            //remotePlayer.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            //Simulation.AddEntity(remotePlayer);

            return player;
        }

        public void LogoutPlayer(Player player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.RemoveEntity(player);
        }
    }
}
