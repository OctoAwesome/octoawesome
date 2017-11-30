using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// ExtensionLoader
    /// </summary>
    public sealed class ExtensionLoader : IExtensionLoader, IExtensionResolver
    {
        private const string SETTINGSKEY = "DisabledExtensions";

        private List<IDefinition> definitions;

        private List<Type> entities;

        private Dictionary<Type, List<Action<Entity>>> entityExtender;

        private List<Action<Simulation>> simulationExtender;

        private List<IMapGenerator> mapGenerators;

        private List<IMapPopulator> mapPopulators;

        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        public List<IExtension> LoadedExtensions { get; private set; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        public List<IExtension> ActiveExtensions { get; private set; }

        private ISettings settings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Current Gamesettings</param>
        public ExtensionLoader(ISettings settings)
        {
            this.settings = settings;
            definitions = new List<IDefinition>();
            entities = new List<Type>();
            entityExtender = new Dictionary<Type, List<Action<Entity>>>();
            simulationExtender = new List<Action<Simulation>>();
            mapGenerators = new List<IMapGenerator>();
            mapPopulators = new List<IMapPopulator>();
            LoadedExtensions = new List<IExtension>();
            ActiveExtensions = new List<IExtension>();

        }

        /// <summary>
        /// Load all Plugins
        /// </summary>
        public void LoadExtensions()
        {
            List<Assembly> assemblies = new List<Assembly>();
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            assemblies.AddRange(LoadAssemblies(dir));

            DirectoryInfo plugins = new DirectoryInfo(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));

            var disabledExtensions = settings.KeyExists(SETTINGSKEY) ? settings.GetArray<string>(SETTINGSKEY) : new string[0];

            List<Type> result = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IExtension).IsAssignableFrom(type))
                        continue;

                    try
                    {
                        IExtension extension = (IExtension)Activator.CreateInstance(type);
                        extension.Register(this);

                        if (disabledExtensions.Contains(type.FullName))
                            LoadedExtensions.Add(extension);
                        else
                            ActiveExtensions.Add(extension);
                    }
                    catch (Exception)
                    {
                        // TODO: Logging
                    }
                }
            }
        }

        private IEnumerable<Assembly> LoadAssemblies(DirectoryInfo directory)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in directory.GetFiles("*.dll"))
            {
                try
                {
                    var assembly = Assembly.LoadFile(file.FullName);
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                    // TODO: Error Handling
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="disabledExtensions">List of Extensions</param>
        public void ApplyExtensions(IList<IExtension> disabledExtensions)
        {
            var types = disabledExtensions.Select(e => e.GetType().FullName).ToArray();
            settings.Set(SETTINGSKEY, types);
        }

        #region Loader Methods

        /// <summary>
        /// Registers a new Definition.
        /// </summary>
        /// <param name="definition">Definition Instance</param>
        public void RegisterDefinition(IDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            // TODO: Replace? Ignore?
            if (definitions.Any(d => d.GetType() == definition.GetType()))
                throw new ArgumentException("Already registered");

            definitions.Add(definition);
        }

        /// <summary>
        /// Removes an existing Definition Type.
        /// </summary>
        /// <typeparam name="T">Definition Type</typeparam>
        public void RemoveDefinition<T>() where T : IDefinition
        {
            var definition = definitions.FirstOrDefault(d => d.GetType() == typeof(T));
            if (definition != null)
                definitions.Remove(definition);
        }

        /// <summary>
        /// Registers a new Entity.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        public void RegisterEntity<T>() where T : Entity
        {
            Type type = typeof(T);
            if (entities.Contains(type))
                throw new ArgumentException("Already registered");

            entities.Add(type);
        }

        /// <summary>
        /// Adds a new Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="extenderDelegate">Extender Delegate</param>
        public void RegisterEntityExtender<T>(Action<Entity> extenderDelegate) where T : Entity
        {
            Type type = typeof(T);
            List<Action<Entity>> list;
            if (!entityExtender.TryGetValue(type, out list))
            {
                list = new List<Action<Entity>>();
                entityExtender.Add(type, list);
            }
            list.Add(extenderDelegate);
        }

        public void RegisterDefaultEntityExtender<T>() where T : Entity 
            => RegisterEntityExtender<T>((e) => e.RegisterDefault());

        /// <summary>
        /// Adds a new Extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        public void RegisterSimulationExtender(Action<Simulation> extenderDelegate)
        {
            simulationExtender.Add(extenderDelegate);
        }

        /// <summary>
        /// Adds a new Map Generator.
        /// </summary>
        public void RegisterMapGenerator(IMapGenerator generator)
        {
            // TODO: Checks
            mapGenerators.Add(generator);
        }

        public void RegisterMapPopulator(IMapPopulator populator)
        {
            mapPopulators.Add(populator);
        }



        /// <summary>
        /// Removes an existing Entity Type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveEntity<T>() where T : Entity
        {
            entities.Remove(typeof(T));
        }

        /// <summary>
        /// Removes an existing Map Generator.
        /// </summary>
        /// <typeparam name="T">Map Generator Type</typeparam>
        public void RemoveMapGenerator<T>(T item) where T : IMapGenerator
        {
            mapGenerators.Remove(item);
        }

        public void RemoveMapPopulator<T>(T item) where T : IMapPopulator
        {
            mapPopulators.Remove(item);
        }

        #endregion

        #region Resolver Methods

        /// <summary>
        /// Extend a Simulation
        /// </summary>
        /// <param name="simulation">Simulation</param>
        public void ExtendSimulation(Simulation simulation)
        {
            foreach (var extender in simulationExtender)
                extender(simulation);
        }

        /// <summary>
        /// Extend a Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void ExtendEntity(Entity entity)
        {
            List<Type> stack = new List<Type>();
            Type t = entity.GetType();
            stack.Add(t);
            do
            {
                t = t.BaseType;
                stack.Add(t);
            }
            while (t != typeof(Entity));
            stack.Reverse();

            foreach (var type in stack)
            {
                List<Action<Entity>> list;
                if (!entityExtender.TryGetValue(type, out list))
                    continue;

                foreach (var item in list)
                    item(entity);
            }
        }

        /// <summary>
        /// Return a List of Definitions
        /// </summary>
        /// <typeparam name="T">Definitiontype</typeparam>
        /// <returns>List</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition
        {
            return definitions.OfType<T>();
        }

        /// <summary>
        /// Return a List of MapGenerators
        /// </summary>
        /// <returns>List of Generators</returns>
        public IEnumerable<IMapGenerator> GetMapGenerator()
        {
            return mapGenerators;
        }

        /// <summary>
        /// Return a List of Populators
        /// </summary>
        /// <returns>List of Populators</returns>
        public IEnumerable<IMapPopulator> GetMapPopulator()
        {
            return mapPopulators;
        }

        #endregion
    }
}
