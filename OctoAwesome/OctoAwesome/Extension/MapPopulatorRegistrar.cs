using System.Collections.Generic;

namespace OctoAwesome
{
    public class MapPopulatorRegistrar : BaseRegistrar<IMapPopulator, IMapPopulator>
    {
        private List<IMapPopulator> mapPopulators;

        public MapPopulatorRegistrar(ISettings settings) : base(settings)
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
        public IReadOnlyCollection<IMapPopulator> GetMapPopulator()
        {
            return mapPopulators;
        }

    }
}
