using OctoAwesome.Entities;
using OctoAwesome.Common;
using System.Collections.Generic;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for a ExtensionResolver
    /// </summary>
    public interface IExtensionResolver
    {
        /// <summary>
        /// Extend a Simulation
        /// </summary>
        /// <param name="simulation">Simulation</param>
        [Obsolete]
        void ExtendSimulation(Simulation simulation);

        /// <summary>
        /// Extend a Entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void ExtendEntity(Entity entity);

        /// <summary>
        /// Return a List of Definitions
        /// </summary>
        /// <typeparam name="T">Definitiontype</typeparam>
        /// <returns>List</returns>
        IEnumerable<T> GetDefinitions<T>() where T : IDefinition;

        /// <summary>
        /// Return a List of MapGenerators
        /// </summary>
        /// <returns>List of Generators</returns>
        IEnumerable<IMapGenerator> GetMapGenerator();

        /// <summary>
        /// Return a List of Populators
        /// </summary>
        /// <returns>List of Populators</returns>
        IEnumerable<IMapPopulator> GetMapPopulator();
    }
}
