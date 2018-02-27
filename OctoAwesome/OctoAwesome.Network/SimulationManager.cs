using engenious;
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
        
        private Simulation simulation;
        private ExtensionLoader extensionLoader;
        private DefinitionManager definitionManager;
        private ResourceManager resourceManager;
        private Settings settings;

        private Thread backgroundThread;
        private object mainLock;

        public SimulationManager()
        {
            mainLock = new object();

            settings = new Settings(); //TODO: Where are the settings?
          
            ExtensionLoader extensionLoader = new ExtensionLoader(settings);
            ExtensionLoader = extensionLoader;
            extensionLoader.LoadExtensions();

            definitionManager = new DefinitionManager(extensionLoader);

            var persistenceManager = new DiskPersistenceManager(extensionLoader, definitionManager, settings);

            resourceManager = new ResourceManager(extensionLoader, definitionManager, settings, persistenceManager);
            extensionLoader.Service = new GameService(definitionManager);
          
            simulation = new Simulation(resourceManager, extensionLoader);
            backgroundThread = new Thread(SimulationLoop)
            {
                Name = "Simulation Loop",
                IsBackground = true
            };
        }

        public void Start()
        {
            IsRunning = true;
            GameTime = new GameTime();

            simulation.NewGame("bla", 42);
            
            backgroundThread.Start();
        }
        
        public void Stop()
        {
            IsRunning = false;
            simulation.ExitGame();
            backgroundThread.Abort();            
        }

        private void SimulationLoop()
        {
            while (IsRunning)
            {
                Simulation.Update(GameTime);
            }
        }
    }
}
