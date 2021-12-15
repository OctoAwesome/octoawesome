using System.Collections.Generic;

namespace OctoAwesome
{
    public class MapGeneratorRegistrar : BaseRegistrar<IMapGenerator>
    {
        private readonly List<IMapGenerator> mapGenerators;

        public MapGeneratorRegistrar(ISettings settings) : base(settings)
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
        public IReadOnlyCollection<IMapGenerator> GetMapGenerator()
        {
            return mapGenerators;
        }
    }
}
