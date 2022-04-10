using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for the extension loader.
    /// </summary>
    public interface IExtensionLoader
    {
        /// <summary>
        /// Gets a list of the loaded extensions.
        /// </summary>
        List<IExtension> LoadedExtensions { get; }

        /// <summary>
        /// Gets a list of the active extensions
        /// </summary>
        List<IExtension> ActiveExtensions { get; }

        /// <summary>
        /// Activate the given list of extensions.
        /// </summary>
        /// <param name="extensions">The list of extensions to activate.</param>
        void ApplyExtensions(IList<IExtension> extensions);

        /// <summary>
        /// Registers a new definition.
        /// </summary>
        /// <param name="definition">The definition type to register.</param>
        void RegisterDefinition(Type definition);

        /// <summary>
        /// Removes an existing Definition Type.
        /// </summary>
        /// <typeparam name="T">The definition type to remove.</typeparam>
        void RemoveDefinition<T>() where T : IDefinition; // TODO: naming Unregister?

        /// <summary>
        /// Registers a type with the required <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        void RegisterSerializationType<T>();

        /// <summary>
        /// Removes an existing entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity to remove.</typeparam>
        void RemoveEntity<T>() where T : ComponentContainer;

        /// <summary>
        /// Registers a new Extender for the given Entity Type.
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="extenderDelegate">Extender Delegate</param>
        void RegisterEntityExtender<T>(Action<ComponentContainer> extenderDelegate) where T : ComponentContainer;

        /// <summary>
        /// Registers the default extender for the given entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity to register the default extender for.</typeparam>
        void RegisterDefaultEntityExtender<T>() where T : ComponentContainer;

        /// <summary>
        /// Register a new extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate">The delegate to register as an extender.</param>
        void RegisterSimulationExtender(Action<Simulation> extenderDelegate);

        /// <summary>
        /// Registers a new map generator.
        /// </summary>
        /// <param name="generator">The map generator to register.</param>
        void RegisterMapGenerator(IMapGenerator generator);

        /// <summary>
        /// Removes an existing map generator.
        /// </summary>
        /// <typeparam name="T">The type of the map generator to remove.</typeparam>
        /// <param name="item">The map generator to remove.</param>
        void RemoveMapGenerator<T>(T item) where T : IMapGenerator;

        /// <summary>
        /// Registers a new map populator.
        /// </summary>
        /// <param name="populator">The map populator to register.</param>
        void RegisterMapPopulator(IMapPopulator populator);

        /// <summary>
        /// Removes an existing map populator.
        /// </summary>
        /// <typeparam name="T">The type of the map populator to remove.</typeparam>
        /// <param name="item">The map populator to remove.</param>
        void RemoveMapPopulator<T>(T item) where T : IMapPopulator;
    }
}
