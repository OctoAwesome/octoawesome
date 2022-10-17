
using OctoAwesome.Location;

using System.Collections.Generic;

namespace OctoAwesome.Extension
{

    /// <summary>
    /// Registrar for map populator extension loading
    /// </summary>
    public class MapPopulatorRegistrar : BaseRegistrar<IMapPopulator>
    {
        private List<IMapPopulator> mapPopulators;

        /// <summary>
        /// Initializes a new instance of the<see cref="MapPopulatorRegistrar" /> class
        /// </summary>
        public MapPopulatorRegistrar()
        {
            mapPopulators = new List<IMapPopulator>();
        }

        /// <inheritdoc/>
        public override void Register(IMapPopulator populator)
        {
            mapPopulators.Add(populator);
        }

        /// <inheritdoc/>
        public override void Unregister(IMapPopulator item)
        {
            mapPopulators.Remove(item);
        }

        /// <summary>
        /// Return a List of Populators
        /// </summary>
        /// <returns>List of Populators</returns>
        public override IReadOnlyCollection<IMapPopulator> Get()
        {
            return mapPopulators;
        }
    }
}
