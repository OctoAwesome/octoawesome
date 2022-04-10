using OctoAwesome.Definitions;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for extension resolvers.
    /// </summary>
    public interface IExtensionResolver
    {
        /// <summary>
        /// Extends a simulation.
        /// </summary>
        /// <param name="simulation">The simulation to extend.</param>
        void ExtendSimulation(Simulation simulation);

        /// <summary>
        /// Extends an entity.
        /// </summary>
        /// <param name="entity">The entity to extend.</param>
        void ExtendEntity(ComponentContainer entity);

        /// <summary>
        /// Gets an enumeration of definitions of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of the definitions to get.</typeparam>
        /// <returns>Enumeration of the definitions of the specified type.</returns>
        IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition;

        /// <summary>
        /// Gets an enumeration of map generators.
        /// </summary>
        /// <returns>Enumeration of the available map generators.</returns>
        IEnumerable<IMapGenerator> GetMapGenerators();

        /// <summary>
        /// Gets an enumeration of map populators.
        /// </summary>
        /// <returns>Enumeration of the available map populator.</returns>
        IEnumerable<IMapPopulator> GetMapPopulators();
    }
}
