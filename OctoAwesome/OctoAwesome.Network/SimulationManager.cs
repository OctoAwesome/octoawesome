using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class SimulationManager
    {
        private Simulation simulation;
        private ExtensionLoader ExtensionLoader;
        private DefinitionManager DefinitionManager;
        private ResourceManager ResourceManager;
        private Settings settings;

        public SimulationManager()
        {
            settings = new Settings(); //TODO: Where are the settings?

            ExtensionLoader extensionLoader = new ExtensionLoader(settings);
            ExtensionLoader = extensionLoader;
            extensionLoader.LoadExtensions();

            DefinitionManager = new DefinitionManager(extensionLoader);
            ResourceManager = new ResourceManager(extensionLoader, DefinitionManager, settings);

            simulation = new Simulation(ResourceManager, extensionLoader);
        }
    }
}
