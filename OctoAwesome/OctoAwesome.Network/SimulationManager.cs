using engenious;
using OctoAwesome.Definitions;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        public bool IsRunning { get; private set; }

        public Simulation Simulation
        {
            get
            {
                lock (mainLock)
                    return simulation;
            }
            set
            {
                lock (mainLock)
                    simulation = value;
            }
        }

        public GameTime GameTime { get; private set; }

        public ResourceManager ResourceManager { get; private set; }
        public GameService Service { get; }

        private Simulation simulation;
        private readonly ExtensionLoader extensionLoader;

        private Task backgroundTask;
        private CancellationTokenSource cancellationTokenSource;

        private readonly ISettings settings;
        private readonly UpdateHub updateHub;
        private readonly object mainLock;

        public SimulationManager(ISettings settings, UpdateHub updateHub)
        {
            mainLock = new object();           

            this.settings = settings;
            this.updateHub = updateHub;


            TypeContainer.Register<ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IExtensionLoader, ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IExtensionResolver, ExtensionLoader>(InstanceBehaviour.Singleton);
            TypeContainer.Register<DefinitionManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<DiskPersistenceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IPersistenceManager, DiskPersistenceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<ResourceManager>(InstanceBehaviour.Singleton);
            TypeContainer.Register<IResourceManager, ResourceManager>(InstanceBehaviour.Singleton);

            extensionLoader = TypeContainer.Get<ExtensionLoader>();
            extensionLoader.LoadExtensions();

            ResourceManager = TypeContainer.Get<ResourceManager>();
            ResourceManager.InsertUpdateHub(updateHub);

            Service = new GameService(ResourceManager);
            simulation = new Simulation(ResourceManager, extensionLoader, Service)
            {
                IsServerSide = true
            };
            
        }

        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            backgroundTask = new Task(SimulationLoop, token, token, TaskCreationOptions.LongRunning);

            //TODO: Load and Save logic for Server (Multiple games etc.....)
            var universe = settings.Get<string>("LastUniverse");

            if (string.IsNullOrWhiteSpace(universe))
            {
                var guid = simulation.NewGame("melmack", new Random().Next().ToString());
                settings.Set("LastUniverse", guid.ToString());
            }
            else
            {
                if (!simulation.TryLoadGame(new Guid(universe)))
                {
                    var guid = simulation.NewGame("melmack", new Random().Next().ToString());
                    settings.Set("LastUniverse", guid.ToString());
                }
            }

            backgroundTask.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            simulation.ExitGame();
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        public IUniverse GetUniverse()
            => ResourceManager.CurrentUniverse;

        public IUniverse NewUniverse()
        {
            throw new NotImplementedException();
        }

        public IPlanet GetPlanet(int planetId)
        {
            var planet = ResourceManager.GetPlanet(planetId);
            planet.UpdateHub = updateHub;
            return planet;
        }

        public IChunkColumn LoadColumn(IPlanet planet, Index2 index2)
            => ResourceManager.LoadChunkColumn(planet, index2);
        public IChunkColumn LoadColumn(int planetId, Index2 index2)
            => LoadColumn(GetPlanet(planetId), index2);

        private void SimulationLoop(object state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                Simulation.Update(GameTime);
            }
        }
    }
}
