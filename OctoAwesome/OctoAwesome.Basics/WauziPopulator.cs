using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.Entities;

namespace OctoAwesome.Basics
{
    public class WauziPopulator : IMapPopulator
    {

        public int Order
        {
            get
            {
                return 11;
            }
        }

        public void Populate(IResourceManager resourcemanager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            WauziEntity wauzi = new WauziEntity(new LocalChunkCache(resourcemanager.GlobalChunkCache,2,1));
            column00.Entities.Add(wauzi);
        }

    }
}
