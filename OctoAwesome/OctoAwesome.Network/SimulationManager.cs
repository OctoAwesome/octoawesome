using engenious;

using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Manager for an OctoAwesome game simulation.
    /// </summary>
    public class SimulationManager
    {
        /// <summary>
        /// Gets a value indicating whether the simulation is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the simulation.
        /// </summary>
        public Simulation Simulation => simulation;

        /// <summary>
        /// Gets the current game time.
        /// </summary>
        public GameTime GameTime { get; private set; }

        /// <summary>
        /// Gets the resource manager used to load resource assets.
        /// </summary>
        public ResourceManager ResourceManager { get; }


        private Simulation simulation;

        private Task backgroundTask;
        private CancellationTokenSource cancellationTokenSource;

        private readonly ISettings settings;
        private readonly UpdateHub updateHub;
        private readonly object mainLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulationManager"/> class.
        /// </summary>
        /// <param name="settings">The game settings for the simulation.</param>
        /// <param name="updateHub">The update hub.</param>
        public SimulationManager(ISettings settings, UpdateHub updateHub)
        {
            mainLock = new object();

            this.settings = settings;
            this.updateHub = updateHub;

            var typeContainer = TypeContainer.Get<ITypeContainer>();

            typeContainer.Register<ExtensionLoader>(InstanceBehavior.Singleton);
            typeContainer.Register<ExtensionService>(InstanceBehavior.Singleton);
            typeContainer.Register<DefinitionManager>(InstanceBehavior.Singleton);
            typeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehavior.Singleton);
            typeContainer.Register<DiskPersistenceManager>(InstanceBehavior.Singleton);
            typeContainer.Register<IPersistenceManager, DiskPersistenceManager>(InstanceBehavior.Singleton);
            typeContainer.Register<ResourceManager>(InstanceBehavior.Singleton);
            typeContainer.Register<IResourceManager, ResourceManager>(InstanceBehavior.Singleton);

            typeContainer.Register<SerializationIdTypeProvider>(InstanceBehavior.Singleton);
            typeContainer.Register<RecipeService, RecipeService>(InstanceBehavior.Singleton);

            var extensionLoader = typeContainer.Get<ExtensionLoader>();
            extensionLoader.LoadExtensions();
            extensionLoader.RegisterExtensions();
            extensionLoader.InstantiateExtensions();

            var extensionService = typeContainer.Get<ExtensionService>();

            ResourceManager = typeContainer.Get<ResourceManager>();

            simulation = new Simulation(ResourceManager, extensionService)
            {
                IsServerSide = true
            };

            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            backgroundTask = new Task(SimulationLoop, token, token, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Start the game simulation.
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();

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

        /// <summary>
        /// Stop the game simulation.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            Simulation.ExitGame();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Gets the currently loaded universe.
        /// </summary>
        /// <returns>The currently loaded universe.</returns>
        public IUniverse GetUniverse()
            => NullabilityHelper.NotNullAssert(ResourceManager.CurrentUniverse);

        /// <summary>
        /// Creates a new universe.
        /// </summary>
        /// <returns>The newly created universe.</returns>
        /// <exception cref="NotImplementedException">Currently not implemented.</exception>
        public IUniverse NewUniverse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a planet by a given id.
        /// </summary>
        /// <param name="planetId">The planet id to get the planet for.</param>
        /// <returns>The loaded or newly generated planet.</returns>
        public IPlanet GetPlanet(int planetId) => ResourceManager.GetPlanet(planetId);

        /// <summary>
        /// Loads a chunk column at a given location for a specified planet.
        /// </summary>
        /// <param name="planet">The planet to load the chunk column from.</param>
        /// <param name="index2">The location to load the chunk column at.</param>
        /// <returns>The loaded chunk column.</returns>
        /// <seealso cref="LoadColumn(int,OctoAwesome.Index2)"/>
        public IChunkColumn LoadColumn(IPlanet planet, Index2 index2)
            => planet.GlobalChunkCache.Subscribe(index2);

        /// <summary>
        /// Loads a chunk column at a given location for a specified planet.
        /// </summary>
        /// <param name="planetId">The id of the planet to load the chunk column from.</param>
        /// <param name="index2">The location to load the chunk column at.</param>
        /// <returns>The loaded chunk column.</returns>
        /// <seealso cref="LoadColumn(IPlanet,OctoAwesome.Index2)"/>
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
