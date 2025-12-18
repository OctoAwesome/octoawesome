using System.Collections.Generic;

using OctoAwesome.Location;


namespace OctoAwesome.Extension
{
    /// <summary>
    /// Registrar for map generator extension loading
    /// </summary>
    public class MapGeneratorRegistrar : IExtensionRegistrar<IMapGenerator>
    {
        private readonly List<IMapGenerator> mapGenerators;

        /// <summary>
        /// Initializes a new instance of the<see cref="MapGeneratorRegistrar" /> class
        /// </summary>
        public MapGeneratorRegistrar()
        {
            mapGenerators = new List<IMapGenerator>();

        }

        /// <summary>
        /// Adds a new Map Generator.
        /// </summary>
        public void Register(IMapGenerator value)
        {
            // TODO: Checks
            mapGenerators.Add(value);
        }

        /// <summary>
        /// Removes an existing Map Generator.
        /// </summary>
        public void Unregister(IMapGenerator value)
        {
            mapGenerators.Remove(value);
        }

        /// <summary>
        /// Return a List of MapGenerators
        /// </summary>
        /// <returns>List of Generators</returns>
        public IReadOnlyCollection<IMapGenerator> Get()
        {
            return mapGenerators;
        }
    }
}
