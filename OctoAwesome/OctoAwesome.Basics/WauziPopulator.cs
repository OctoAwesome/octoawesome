using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics
{
    public class WauziPopulator : IMapPopulator
    {

        Random r = new Random();

        public int Order
        {
            get
            {
                return 11;
            }
        }

        int ispop = 10;

        public void Populate(IResourceManager resourcemanager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            if (ispop-- <= 0)
                return;

            WauziEntity wauzi = new WauziEntity(new LocalChunkCache(resourcemanager.GlobalChunkCache,2,1));

            var x = r.Next(0, Chunk.CHUNKSIZE_X/2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y/2);

            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, new Index3(x+column01.Index.X*Chunk.CHUNKSIZE_X, y + column01.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0, 0)) };
            wauzi.Components.AddComponent(position);
            column00.Entities.Add(wauzi);
        }

    }
}
