using OctoAwesome.Caching;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Network.Request;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Manages updates received and to be sent over network on client side.
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
        private readonly ConcurrentDictionary<uint, Awaiter> packages = new();
        private readonly Pool<OfficialCommandDTO> requestPool;

        private readonly Relay<object> simulationRelay;
        private readonly Relay<object> chunkChannel;
        private readonly PackageActionHub packageActionHub;

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

            simulationRelay = new Relay<object>();
            chunkChannel = new Relay<object>();
            packageActionHub = new PackageActionHub(logger, typeContainer);

            hubSubscription
                = updateHub
                .ListenOnNetwork()
                .Subscribe(OnNext, error => logger.Error(error.Message, error));

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
            chunkSource = updateHub.AddSource(chunkChannel, DefaultChannels.Chunk);

            clientSubscription = client.Packages.Subscribe(OnNext, OnError);
            requestPool = new();

        }

        /// <summary>
        /// Gets called when a new package is received.
        /// </summary>
        /// <param name="package">The received package.</param>
        public void OnNext(Package package)
        {

            if ((package.PackageFlags & PackageFlags.Response) > 0)
            {
                logger.Trace($"Package with id:{package.UId} and Flags: {package.PackageFlags}");

                if (packages.TryRemove(package.UId, out var awaiter))
                {
                    if (!awaiter.TrySetResult(package.Payload))
                        logger.Warn($"Awaiter can not set result package {package.UId}");
                }
                else
                {
                    logger.Error($"Got response for package without having a request with id {package.UId}");
                }
            }
            else
            {
                packageActionHub.Dispatch(package, client);
            }
        }

        private void OnNext(PushInfo value)
        {
            if (value.Notification is not SerializableNotification notification)
            {
                SendGeneric(value);
                return;
            }

            if (notification.SenderId == client.Id)
                return;
            SendGeneric(value);
        }

        private void SendGeneric(PushInfo value)
        {
            if (value.Notification is not ISerializable ser)
                return;

            var package = packagePool.Rent();

            using (var memoryStream = Serializer.Manager.GetStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(ser.GetType().SerializationId());
                binaryWriter.Write(value.Channel);
                ser.Serialize(binaryWriter);

                package.PackageFlags = PackageFlags.Notification;
                package.Payload = memoryStream.ToArray();
                _ = client.SendPackageAndRelease(package);
            }
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
            chunkChannel.Dispose();
            simulationRelay.Dispose();
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

            _ = client.SendPackageAndRelease(package);
            return awaiter;
        }

        public Awaiter SendAndAwait(byte[] serialized, PackageFlags flags = PackageFlags.None)
        {
            var package = packagePool.Rent();
            package.Payload = serialized;
            package.PackageFlags = flags;

            var awaiter = GetAwaiter(package.UId);
            _ = client.SendPackageAndRelease(package);
            return awaiter;
        }
        /// <summary>
        /// Serializes the serializable with see <see cref="Serializer.SerializeNetwork(ISerializable)"/> method and therefore write the SerializationId itself
        /// </summary>
        /// <param name="serializable"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Awaiter SendAndAwait(ISerializable serializable, PackageFlags flags = PackageFlags.None)
        {
            return SendAndAwait(Serializer.SerializeNetwork(serializable), flags);
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