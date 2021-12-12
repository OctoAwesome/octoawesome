using engenious;
using OctoAwesome.Definitions;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        public bool IsRunning { get; private set; }
        
        public Simulation Simulation { get; }
        
        public GameTime GameTime { get; private set; }
        
        public ResourceManager ResourceManager { get; }
        
        public GameService Service { get; }

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
            TypeContainer.Register<ExtensionLoader>(InstanceBehavior.Singleton);
            TypeContainer.Register<IExtensionLoader, ExtensionLoader>(InstanceBehavior.Singleton);
            TypeContainer.Register<IExtensionResolver, ExtensionLoader>(InstanceBehavior.Singleton);
            TypeContainer.Register<DefinitionManager>(InstanceBehavior.Singleton);
            TypeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehavior.Singleton);
            TypeContainer.Register<DiskPersistenceManager>(InstanceBehavior.Singleton);
            TypeContainer.Register<IPersistenceManager, DiskPersistenceManager>(InstanceBehavior.Singleton);
            TypeContainer.Register<ResourceManager>(InstanceBehavior.Singleton);
            TypeContainer.Register<IResourceManager, ResourceManager>(InstanceBehavior.Singleton);

            extensionLoader = TypeContainer.Get<ExtensionLoader>();
            extensionLoader.LoadExtensions();

            ResourceManager = TypeContainer.Get<ResourceManager>();

            Service = new GameService(ResourceManager);
            Simulation = new Simulation(ResourceManager, extensionLoader, Service)
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
                var guid = Simulation.NewGame("melmack", new Random().Next().ToString());
                settings.Set("LastUniverse", guid.ToString());
            }
            else
            {
                if (!Simulation.TryLoadGame(new Guid(universe)))
                {
                    var guid = Simulation.NewGame("melmack", new Random().Next().ToString());
                    settings.Set("LastUniverse", guid.ToString());
                }
            }

            backgroundTask.Start();
        }
        public void Stop()
        {
            IsRunning = false;
            Simulation.ExitGame();
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

        private void SimulationLoop(object? state)
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
