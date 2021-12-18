using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Manages updates received and to be sent over network.
    /// </summary>
    public class NetworkUpdateManager : IDisposable
    {
        private readonly Client client;
        private readonly ILogger logger;
        private readonly IDisposable hubSubscription;
        private readonly IDisposable simulationSource;
        private readonly IDisposable chunkSource;
        private readonly IDisposable clientSubscription;
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IPool<BlockChangedNotification> blockChangedNotificationPool;
        private readonly IPool<BlocksChangedNotification> blocksChangedNotificationPool;
        private readonly PackagePool packagePool;

        private readonly Relay<Notification> simulation;
        private readonly Relay<Notification> chunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkUpdateManager"/> class.
        /// </summary>
        /// <param name="client">The network client that is connected to the remote server.</param>
        /// <param name="updateHub">The update hub to receive updates from.</param>
        public NetworkUpdateManager(Client client, IUpdateHub updateHub)
        {
            this.client = client;

            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkUpdateManager));
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            blocksChangedNotificationPool = TypeContainer.Get<IPool<BlocksChangedNotification>>();
            packagePool = TypeContainer.Get<PackagePool>();

            simulation = new Relay<Notification>();
            chunk = new Relay<Notification>();

            hubSubscription
                = updateHub
                .ListenOn(DefaultChannels.Network)
                .Subscribe(OnNext, error => logger.Error(error.Message, error));

            simulationSource = updateHub.AddSource(simulation, DefaultChannels.Simulation);
            chunkSource = updateHub.AddSource(chunk, DefaultChannels.Chunk);

            clientSubscription = client.Packages.Subscribe(package => OnNext(package), err => OnError(err));

        }

        /// <summary>
        /// Gets called when a new package is received.
        /// </summary>
        /// <param name="package">The received package.</param>
        public void OnNext(Package package)
        {
            switch (package.OfficialCommand)
            {
                case OfficialCommand.EntityNotification:
                    var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, package.Payload);
                    simulation.OnNext(entityNotification);
                    entityNotification.Release();
                    break;
                case OfficialCommand.ChunkNotification:
                    var notificationType = (BlockNotificationType)package.Payload[0];
                    Notification chunkNotification;

                    switch (notificationType)
                    {
                        case BlockNotificationType.BlockChanged:
                            chunkNotification = Serializer.DeserializePoolElement(blockChangedNotificationPool, package.Payload);
                            break;
                        case BlockNotificationType.BlocksChanged:
                            chunkNotification = Serializer.DeserializePoolElement(blocksChangedNotificationPool, package.Payload);
                            break;
                        default:
                            throw new NotSupportedException($"This Type is not supported: {notificationType}");
                    }

                    chunk.OnNext(chunkNotification);
                    chunkNotification.Release();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Gets called when a new notification is received.
        /// </summary>
        /// <param name="value">The received notification.</param>
        public void OnNext(Notification value)
        {
            ushort command;
            byte[] payload;
            switch (value)
            {
                case EntityNotification entityNotification:
                    command = (ushort)OfficialCommand.EntityNotification;
                    payload = Serializer.Serialize(entityNotification);
                    break;
                case BlockChangedNotification chunkNotification:
                    command = (ushort)OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(chunkNotification);
                    break;
                default:
                    return;
            }
            var package = packagePool.Rent();
            package.Command = command;
            package.Payload = payload;
            client.SendPackageAndRelease(package);
        }

        /// <summary>
        /// Gets called when an error occured while receiving.
        /// </summary>
        /// <param name="error">The error that occured.</param>
        public void OnError(Exception error)
        {
            logger.Error(error.Message, error);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            hubSubscription?.Dispose();
            simulationSource?.Dispose();
            chunkSource?.Dispose();
            chunk?.Dispose();
            simulation?.Dispose();
            clientSubscription?.Dispose();
        }
    }
}