using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Manages updates received and to be sent over network.
    /// </summary>
    public class NetworkPackageManager : IDisposable
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
        private readonly IPool<Awaiter> awaiterPool;
        private readonly ConcurrentDictionary<uint, Awaiter> packages;

        private readonly Relay<object> simulation;
        private readonly Relay<object> chunk;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPackageManager"/> class.
        /// </summary>
        /// <param name="client">The network client that is connected to the remote server.</param>
        /// <param name="updateHub">The update hub to receive updates from.</param>
        public NetworkPackageManager(Client client, IUpdateHub updateHub, ITypeContainer typeContainer)
        {
            this.client = client;

            logger = (typeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkPackageManager));
            entityNotificationPool = typeContainer.Get<IPool<EntityNotification>>();
            blockChangedNotificationPool = typeContainer.Get<IPool<BlockChangedNotification>>();
            blocksChangedNotificationPool = typeContainer.Get<IPool<BlocksChangedNotification>>();
            packagePool = typeContainer.Get<PackagePool>();
            awaiterPool = TypeContainer.Get<IPool<Awaiter>>();

            simulation = new Relay<object>();
            chunk = new Relay<object>();

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

            if ((package.PackageFlags & PackageFlags.Response) > 0)
            {
                //var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, package.Payload);

                logger.Trace($"Package with id:{package.UId} and Flags: {package.PackageFlags}");

                if (packages.TryRemove(package.UId, out var awaiter))
                {
                    if (!awaiter.TrySetResult(package.Payload))
                        logger.Warn($"Awaiter can not set result package {package.UId}");
                }
                return;
            }


            //Is Compressed?

            //Is Array?

            //Get Type Info
            //Is Poolable? And maybe get Pool
            //  Deserialize Pool Element

            //Push into relays

        }

        /// <summary>
        /// Gets called when a new notification is received.
        /// </summary>
        /// <param name="value">The received notification.</param>
        public void OnNext(object value)
        {
            //object is ISerializable because of serialization?
            if (value is not ISerializable serializable)
                return;

            //Fire and forget

            var package = packagePool.Rent();
            package.Payload = Serializer.Serialize(serializable);
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
            hubSubscription.Dispose();
            simulationSource.Dispose();
            chunkSource.Dispose();
            chunk.Dispose();
            simulation.Dispose();
            clientSubscription.Dispose();
        }

        public Awaiter SendAndAwait(Stream stream, int offset, int length, PackageFlags flags = PackageFlags.None)
        {
            var package = packagePool.Rent();
            var buffer = ArrayPool<byte>.Shared.Rent(length);
            stream.ReadExactly(buffer, offset, length);
            package.Payload = buffer;
            package.Length = length;

            package.PackageFlags = flags;

            var awaiter = GetAwaiter(package.UId);
            client.SendPackageAndRelease(package);
            return awaiter;
        }

        public Awaiter SendAndAwait(byte[] serialized, PackageFlags flags = PackageFlags.None)
        {
            var package = packagePool.Rent();
            package.Payload = serialized;
            package.PackageFlags = flags;

            var awaiter = GetAwaiter(package.UId);
            client.SendPackageAndRelease(package);
            return awaiter;
        }

        private Awaiter GetAwaiter(uint packageUId)
        {
            var awaiter = awaiterPool.Rent();

            if (!packages.TryAdd(packageUId, awaiter))
            {
                logger.Error($"Awaiter for package {packageUId} could not be added");
            }

            return awaiter;
        }
    }
}