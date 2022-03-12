
using OctoAwesome.Location;

using System.Collections.Generic;

namespace OctoAwesome.Extension
{
    public class MapPopulatorRegistrar : BaseRegistrar<IMapPopulator>
    {
        private List<IMapPopulator> mapPopulators;

        public MapPopulatorRegistrar()
        {
            mapPopulators = new List<IMapPopulator>();
        }

        public override void Register(IMapPopulator populator)
        {
            mapPopulators.Add(populator);
        }

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
