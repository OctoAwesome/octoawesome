using OctoAwesome.Extension;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// ExtensionLoader
    /// </summary>
    public sealed class ExtensionLoader
    {
        private const string SETTINGSKEY = "DisabledExtensions";

        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        public List<IExtension> LoadedExtensions { get; private set; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        public List<IExtension> ActiveExtensions { get; private set; }


        private readonly ISettings settings;
        private readonly ExtensionService extensionService;
        private readonly ITypeContainer typeContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionLoader"/> class.
        /// </summary>
        /// <param name="extensionService">The extension service..</param>
        /// <param name="typeContainer">The type container to manage types.</param>
        /// <param name="settings">Current Game settings.</param>
        public ExtensionLoader(ExtensionService extensionService, ITypeContainer typeContainer, ISettings settings)
        {
            this.typeContainer = typeContainer;
            this.settings = settings;
            this.extensionService = extensionService;

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

            DirectoryInfo dir = new(Path.GetDirectoryName(tempAssembly!.Location)!);
            assemblies.AddRange(LoadAssemblies(dir));

            DirectoryInfo plugins = new(Path.Combine(dir.FullName, "plugins"));
            if (plugins.Exists)
                assemblies.AddRange(LoadAssemblies(plugins));

            var disabledExtensions = settings.KeyExists(SETTINGSKEY)
                ? settings.GetArray<string>(SETTINGSKEY)
                : Array.Empty<string>();

            foreach (var assembly in assemblies)
            {
                var types = assembly
                    .GetTypes();

                foreach (var type in types)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }

                    ExtensionInformation information;

                    if (typeof(IExtensionExtender).IsAssignableFrom(type))
                    {
                        information = new ExtensionInformation((IExtensionExtender)typeContainer.GetUnregistered(type));
                    }
                    else if (typeof(IExtensionRegistrar).IsAssignableFrom(type))
                    {
                        information = new ExtensionInformation((IExtensionRegistrar)typeContainer.GetUnregistered(type));
                    }
                    else
                    {
                        continue;
                    }

                    extensionService.AddExtensionLoader(information);
                }
            }

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
                            IExtension extension = (IExtension)typeContainer.GetUnregistered(type);

                            extension.Register(typeContainer);
                            extension.Register(extensionService);

                            if (disabledExtensions.Contains(type.FullName))
                                LoadedExtensions.Add(extension);
                            else
                                ActiveExtensions.Add(extension);

                            var extensionInformation = new ExtensionInformation(extension);
                            extensionService.AddExtensionLoader(extensionInformation);
                        }
                        catch
                        {
                            // TODO: Logging
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="disabledExtensions">List of Extensions</param>
        public void Apply(IList<IExtension> disabledExtensions)
        {
            var types = disabledExtensions.Select(e =>
                                                  {
                                                      var typeName = e.GetType().FullName;
                                                      Debug.Assert(typeName != null, nameof(typeName) + " != null");
                                                      return typeName;
                                                  }).ToArray();
            settings.Set(SETTINGSKEY, types);
        }

        private IEnumerable<Assembly> LoadAssemblies(DirectoryInfo directory)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var file in directory.GetFiles("*.dll", SearchOption.AllDirectories))
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
