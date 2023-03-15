using engenious;

using OctoAwesome.Basics.Entities;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

using System;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Map populater to populate the world with wauzi mob entities.
    /// </summary>
    public class WauziPopulator : IMapPopulator, IDisposable
    {
        private readonly Random r = new Random();
        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSubscription;

        /// <inheritdoc />
        public int Order => 11;

        private int ispop = 3;
        private bool disposedValue;
        /// <summary>
        /// Initializes a new instance of the <see cref="WauziPopulator"/> class.
        /// </summary>
        public WauziPopulator(IResourceManager resManager)
        {

            simulationRelay = new Relay<Notification>();

            simulationSubscription
                = resManager
                .UpdateHub
                .AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        /// <inheritdoc />
        public void Populate(IResourceManager resourcemanager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11)
        {
            //HACK: Activate Wauzi
            //return;

            if (ispop-- <= 0)
                return;

            WauziEntity wauzi = new WauziEntity();

            var x = r.Next(0, Chunk.CHUNKSIZE_X / 2);
            var y = r.Next(0, Chunk.CHUNKSIZE_Y / 2);

            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, new Index3(x + column00.Index.X * Chunk.CHUNKSIZE_X, y + column00.Index.Y * Chunk.CHUNKSIZE_Y, 200), new Vector3(0, 0, 0)) };
            wauzi.Components.AddIfTypeNotExists(position);

            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add, wauzi));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">If currently in <see cref="Dispose()"/> call</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    simulationSubscription.Dispose();
                }

                disposedValue = true;
            }
        }


        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
