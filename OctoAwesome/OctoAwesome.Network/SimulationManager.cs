using engenious;
using OctoAwesome.Definitions;
using OctoAwesome.Extension;
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

            var typeContainer = TypeContainer.Get<ITypeContainer>();

            typeContainer.Register<ExtensionLoader>(InstanceBehaviour.Singleton);
            typeContainer.Register<ExtensionService>(InstanceBehaviour.Singleton);
            typeContainer.Register<DefinitionManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<DiskPersistenceManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPersistenceManager, DiskPersistenceManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<ResourceManager>(InstanceBehaviour.Singleton);
            typeContainer.Register<IResourceManager, ResourceManager>(InstanceBehaviour.Singleton);

            var extensionLoader = typeContainer.Get<ExtensionLoader>();
            extensionLoader.LoadExtensions();

            var extensionService = typeContainer.Get<ExtensionService>();

            ResourceManager = typeContainer.Get<ResourceManager>();

            Service = new GameService(ResourceManager);
            simulation = new Simulation(ResourceManager, extensionService, Service)
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

        public IPlanet GetPlanet(int planetId) => ResourceManager.GetPlanet(planetId);

        public IChunkColumn LoadColumn(IPlanet planet, Index2 index2)
            => planet.GlobalChunkCache.Subscribe(index2);
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
