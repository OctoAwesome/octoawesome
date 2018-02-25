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
    // TODO: populator system überarbeiten?
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

            WauziEntity wauzi = new WauziEntity();

            var x = r.Next(0, Chunk.CHUNKSIZE_X/2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y/2);

            // TODO: der code wird immer redundanter :D
            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, new Index3(x+column00.Index.X*Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0, 0)) };
            wauzi.Components.AddComponent(position);
            // TODO: warum wird das wauzi nicht direkt auf dem planeten gespritz...
            // und findet dann seinen weg über die postion zum chunck ?
            // oder noch besser in die simulation
            column00.Entities.Add(wauzi);
        }

    }
}
