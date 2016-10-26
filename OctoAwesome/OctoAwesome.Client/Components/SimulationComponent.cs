using OctoAwesome.Runtime;
using System;
using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private Simulation Simulation { get; set; }
        
        public SimulationComponent(Game game) : base(game) { }

        public EntityManager EntityManager;

        public Guid NewGame(string name, int? seed = null)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }
            
            EntityManager = new EntityManager();


            
            Simulation = new Simulation(ResourceManager.Instance, EntityManager);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            EntityManager = new EntityManager();

            

            Simulation = new Simulation(ResourceManager.Instance, EntityManager);
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

        public ActorHost InsertPlayer(Entity player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            return new ActorHost(player);
            //Simulation.EntityManager.NewEntity();

            //Simulation.AddEntity(player); // InsertPlayer(player);
            return null;
        }

        public void RemovePlayer(ActorHost host)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            EntityManager.RemoveEntity(host.PlayerEntity);

            //Simulation.RemovePlayer(host);
            //Simulation.RemoveEntity(host.Player);
        }
    }
}
