using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Runtime
{
    public sealed class ExtensionLoader : IExtensionLoader, IExtensionResolver
    {
        private List<IDefinition> definitions;

        private List<Type> entities;

        private Dictionary<Type, List<Action<Entity>>> entityExtender;

        private List<Action<Simulation>> simulationExtender;

        public ExtensionLoader()
        {
            definitions = new List<IDefinition>();
            entities = new List<Type>();
            entityExtender = new Dictionary<Type, List<Action<Entity>>>();
            simulationExtender = new List<Action<Simulation>>();
        }

        public void LoadExtensions()
        {
            List<Assembly> assemblies = new List<Assembly>();
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            assemblies.AddRange(LoadAssemblies(dir));

            DirectoryInfo plugins = new DirectoryInfo(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));


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
                    }
                    catch (Exception ex)
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
                catch (Exception ex)
                {
                    // TODO: Error Handling
                }
            }
            return assemblies;
        }

        #region Loader Methods

        public void RegisterDefinition(IDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            // TODO: Replace? Ignore?
            if (definitions.Any(d => d.GetType() == definition.GetType()))
                throw new ArgumentException("Already registered");

            definitions.Add(definition);
        }

        public void RemoveDefinition<T>() where T : IDefinition
        {
            var definition = definitions.FirstOrDefault(d => d.GetType() == typeof(T));
            if (definition != null)
                definitions.Remove(definition);
        }

        public void RegisterEntity<T>() where T : Entity
        {
            Type type = typeof(T);
            if (entities.Contains(type))
                throw new ArgumentException("Already registered");

            entities.Add(type);
        }

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

        /// <summary>
        /// Adds a new Extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        public void RegisterSimulationExtender(Action<Simulation> extenderDelegate)
        {
            simulationExtender.Add(extenderDelegate);
        }

        public void RegisterMapGenerator<T>() where T : IMapGenerator
        {
            throw new NotImplementedException();
        }

        public void RemoveEntity<T>() where T : Entity
        {
            throw new NotImplementedException();
        }

        public void RemoveMapGenerator<T>() where T : IMapGenerator
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Resolver Methods

        public void ExtendSimulation(Simulation simulation)
        {
            foreach (var extender in simulationExtender)
                extender(simulation);
        }

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
                    return;

                foreach (var item in list)
                    item(entity);
            }
        }

        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition
        {
            return definitions.OfType<T>();
        }

        #endregion
    }
}
