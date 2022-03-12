
using OctoAwesome.Location;

using System.Collections.Generic;

namespace OctoAwesome.Extension
{
    public class MapGeneratorRegistrar : BaseRegistrar<IMapGenerator>
    {
        private readonly List<IMapGenerator> mapGenerators;

        public MapGeneratorRegistrar()
        {
            mapGenerators = new List<IMapGenerator>();

        }

        /// <summary>
        /// Adds a new Map Generator.
        /// </summary>
        public override void Register(IMapGenerator value)
        {
            // TODO: Checks
            mapGenerators.Add(value);
        }

        /// <summary>
        /// Removes an existing Map Generator.
        /// </summary>
        /// <typeparam name="T">Map Generator Type</typeparam>
        public override void Unregister(IMapGenerator value)
        {
            mapGenerators.Remove(value);
        }

        /// <summary>
        /// Return a List of MapGenerators
        /// </summary>
        /// <returns>List of Generators</returns>
        public override IReadOnlyCollection<IMapGenerator> Get()
        {
            return mapGenerators;
        }
    }
}
