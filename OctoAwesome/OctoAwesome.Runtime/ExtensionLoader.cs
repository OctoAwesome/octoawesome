using OctoAwesome.Definitions;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// ExtensionLoader
    /// </summary>
    public sealed class ExtensionLoader : IExtensionLoader
    {
        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        public List<IExtension> LoadedExtensions { get; private set; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        public List<IExtension> ActiveExtensions { get; private set; }


        private readonly ISettings settings;

        private readonly ITypeContainer typeContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionLoader"/> class.
        /// </summary>
        /// <param name="settings">Current Gamesettings</param>
        public ExtensionLoader(ITypeContainer typeContainer, ISettings settings)
        {
            this.typeContainer = typeContainer;
            this.settings = settings;
            LoadedExtensions = new List<IExtension>();
            ActiveExtensions = new List<IExtension>();

        }

        /// <summary>
        /// Load all Plugins.
        /// </summary>
        public void LoadExtensions()
        {
            List<Assembly> assemblies = new();
            var tempAssembly = Assembly.GetEntryAssembly();

            if (tempAssembly == null)
                tempAssembly = Assembly.GetAssembly(GetType());

            DirectoryInfo dir = new(Path.GetDirectoryName(tempAssembly!.Location!)!);
            assemblies.AddRange(LoadAssemblies(dir));

            DirectoryInfo plugins = new(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));

            var disabledExtensions = settings.KeyExists(IExtensionLoader.SETTINGSKEY) 
                ? settings.GetArray<string>(IExtensionLoader.SETTINGSKEY) 
                : Array.Empty<string>();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetTypes();

                foreach (var type in types)
                {
                    if (typeof(IExtension).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        try
                        {
                            IExtension extension = (IExtension)Activator.CreateInstance(type)!;

                            extension.Register(typeContainer);
                            extension.Register(this);

                            if (disabledExtensions.Contains(type.FullName))
                                LoadedExtensions.Add(extension);
                            else
                                ActiveExtensions.Add(extension);
                        }
                        catch
                        {
                            // TODO: Logging
                        }
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
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    assemblies.Add(assembly);
                }
                catch (Exception)
                {
                    // TODO: Error Handling
                }
            }
            return assemblies;
        }

    }
}
