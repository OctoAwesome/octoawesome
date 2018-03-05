﻿using System;
using engenious;
using OctoAwesome.Basics.Entities;

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

            wauzi.SetPosition(new Coordinate(planet.Id, 
                new Index3(x + column00.Index.X * Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), 
                new Vector3(0, 0, 0)), 0f);
            // TODO: warum wird das wauzi nicht direkt auf dem planeten gespawnt...
            // oder noch besser in die simulation
            // und findet dann seinen weg über die postion zum chunck ?
            column00.Entities.Add(wauzi);
        }

    }
}
