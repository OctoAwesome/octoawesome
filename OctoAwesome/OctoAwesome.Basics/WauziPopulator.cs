using System;
using engenious;
using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Map populater to populate the world with wauzi mob entities.
    /// </summary>
    public class WauziPopulator : IMapPopulator
    {
        private readonly Random r = new();

        /// <inheritdoc />
        public int Order => 11;

        int ispop = 10;

        /// <inheritdoc />
        public void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            // TODO: HACK: Deactivate Wauzi
            return;

            if (ispop-- <= 0)
                return;

            WauziEntity wauzi = new WauziEntity();

            var x = r.Next(0, Chunk.CHUNKSIZE_X / 2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y / 2);

            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, new Index3(x + column00.Index.X * Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0, 0)) };
            wauzi.Components.AddComponent(position);
            column00.Add(wauzi);
        }

    }
}
