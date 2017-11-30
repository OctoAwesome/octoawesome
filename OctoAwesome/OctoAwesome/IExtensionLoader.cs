using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for the Extension Loader.
    /// </summary>
    public interface IExtensionLoader
    {
        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        List<IExtension> LoadedExtensions { get; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        List<IExtension> ActiveExtensions { get; }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="extensions">List of Extensions</param>
        void ApplyExtensions(IList<IExtension> extensions);

        /// <summary>
        /// Registers a new Definition.
        /// </summary>
        /// <param name="definition">Definition Instance</param>
        void RegisterDefinition(IDefinition definition);

        /// <summary>
        /// Removes an existing Definition Type.
        /// </summary>
        /// <typeparam name="T">Definition Type</typeparam>
        void RemoveDefinition<T>() where T : IDefinition;

        /// <summary>
        /// Registers a new Entity.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        void RegisterEntity<T>() where T : Entity;

        /// <summary>
        /// Removes an existing Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        void RemoveEntity<T>() where T : Entity;

        /// <summary>
        /// Adds a new Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="extenderDelegate">Extender Delegate</param>
        void RegisterEntityExtender<T>(Action<Entity> extenderDelegate) where T : Entity;

        /// <summary>
        /// Adds the Default Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        void RegisterDefaultEntityExtender<T>() where T : Entity;

        /// <summary>
        /// Adds a new Extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        void RegisterSimulationExtender(Action<Simulation> extenderDelegate);

        /// <summary>
        /// Adds a new Map Generator.
        /// </summary>
        void RegisterMapGenerator(IMapGenerator generator);

        /// <summary>
        /// Removes an existing Map Generator.
        /// </summary>
        /// <typeparam name="T">Map Generator Type</typeparam>
        void RemoveMapGenerator<T>(T item) where T : IMapGenerator;

        void RegisterMapPopulator(IMapPopulator populator);

        void RemoveMapPopulator<T>(T item) where T : IMapPopulator; 
    }
}
