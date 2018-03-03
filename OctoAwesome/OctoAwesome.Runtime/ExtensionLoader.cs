using OctoAwesome.Entities;
using OctoAwesome.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OctoAwesome.CodeExtensions;

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

        private Dictionary<Type, List<Action<Entity, IGameService>>> entityExtender;

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
            entityExtender = new Dictionary<Type, List<Action<Entity, IGameService>>>();
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
            var tempAssembly = Assembly.GetEntryAssembly();

            if (tempAssembly == null)
                tempAssembly = Assembly.GetAssembly(GetType());

            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(tempAssembly.Location));
            assemblies.AddRange(LoadAssemblies(dir));

            DirectoryInfo plugins = new DirectoryInfo(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));

            var disabledExtensions = settings.KeyExists(SETTINGSKEY) ? settings.GetArray<string>(SETTINGSKEY) : new string[0];
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        try
                        {
                            // added isabstract check...
                            if (type.IsAbstract || !typeof(IExtension).IsAssignableFrom(type))
                                continue;
                            IExtension extension = (IExtension) Activator.CreateInstance(type);
                            extension.LoadDefinitions(this);
                            extension.Extend(this);

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
                catch(Exception)
                {

                }
            }
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

        #region Loader Methods

        /// <summary>
        /// Load definitions from resouce
        /// </summary>
        /// <param name="embeddedresource">Path of resource file -> [Assembly.Namespace.name.txt|.xml]</param>
        public void LoadDefinitionsFromResource(string embeddedresource)
        {
            //try
            //{
            //    string[] splitted = embeddedresource.Split(StringSplitOptions.RemoveEmptyEntries, '.');
            //    if (splitted.Last().Equals("xml"))
            //    {
            //        Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(embeddedresource);
            //        DefinitionResolverXML resolver = new DefinitionResolverXML();
            //        resolver.Resolve(stream, this);
            //    }
            //}
            //catch (Exception exception)
            //{
            //    //TODO: loggen
            //}
        }

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
        public void RegisterEntityExtender<T>(Action<Entity, IGameService> extenderDelegate) where T : Entity
        {
            Type type = typeof(T);
            if (!entityExtender.TryGetValue(type, out List<Action<Entity, IGameService>> list))
            {
                list = new List<Action<Entity, IGameService>>();
                entityExtender.Add(type, list);
            }
            list.Add(extenderDelegate);
        }

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

        /// <summary>
        /// Register an <see cref="IMapPopulator"/>
        /// </summary>
        /// <param name="populator"></param>
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

        /// <summary>
        /// Removes an existing Map Populater.
        /// </summary>
        /// <typeparam name="T">Populater Type</typeparam>
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
        /// <param name="service">Game services</param>
        public void ExtendEntity(Entity entity, IGameService service)
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
                if (entityExtender.TryGetValue(type, out List<Action<Entity, IGameService>> list))
                    list.ForEach(a => a.Invoke(entity, service));

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
