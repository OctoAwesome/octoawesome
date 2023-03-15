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
        private readonly ConcurrentDictionary<uint, Awaiter> packages = new();
        private readonly Pool<OfficialCommandDTO> requestPool;

        private readonly Relay<object> simulationRelay;
        private readonly Relay<object> chunkChannel;

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

            hubSubscription
                = updateHub
                .ListenOn(DefaultChannels.Network)
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
                //var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, package.Payload);

                logger.Trace($"Package with id:{package.UId} and Flags: {package.PackageFlags}");

                if (packages.TryRemove(package.UId, out var awaiter))
                {
                    if (!awaiter.TrySetResult(package.Payload))
                        logger.Warn($"Awaiter can not set result package {package.UId}");
                }
                return;
            }

            using var ms = Serializer.Manager.GetStream(package.Payload);
            using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal) : ms;
            using var br = new BinaryReader(s);

            var desId = br.ReadUInt64();

            if (desId == typeof(OfficialCommandDTO).SerializationId())
            {
                //Is Array, currently only non array support?
                if ((package.PackageFlags & PackageFlags.Array) == 0)
                {
                    Notification? notification = null;

                    var dto = OfficialCommandDTO.DeserializeAndCreate(br);
                    switch (dto.Command)
                    {
                        case OfficialCommand.EntityNotification:
                            notification = Serializer.DeserializePoolElement<EntityNotification>(entityNotificationPool, dto.Data);
                            if (notification is not null)
                                simulationRelay.OnNext(notification);

                            break;
                        case OfficialCommand.ChunkNotification:
                            var notificationType = (BlockNotificationType)dto.Data[0];
                            switch (notificationType)
                            {
                                case BlockNotificationType.BlockChanged:
                                    notification = Serializer.DeserializePoolElement(blockChangedNotificationPool, dto.Data);
                                    break;
                                case BlockNotificationType.BlocksChanged:
                                    notification = Serializer.DeserializePoolElement(blocksChangedNotificationPool, dto.Data);
                                    break;
                            }
                            if (notification is not null)
                                chunkChannel.OnNext(notification);

                            break;
                    }
                }
            }
            //TODO Notification deserialization and pushing (2023_02_08)



            //Get Type Info
            //Is Poolable? And maybe get Pool
            //  Deserialize Pool Element

            //Push into relays

        }

        private void OnNext(object value)
        {
            if (value is not Notification notification)
                return;


            OfficialCommand command;
            byte[] payload;
            switch (value)
            {
                case EntityNotification entityNotification:
                    command = OfficialCommand.EntityNotification;
                    BuildAndSendPackage(entityNotification, command);
                    break;
                case BlocksChangedNotification _:
                case BlockChangedNotification _:
                    command = OfficialCommand.ChunkNotification;
                    BuildAndSendPackage(GenericCaster<object, SerializableNotification>.Cast(value), command);
                    break;
                default:
                    return;
            }

        }

        private void BuildAndSendPackage(SerializableNotification data, OfficialCommand officialCommand)
        {
            var package = packagePool.Rent();
            using (var memoryStream = Serializer.Manager.GetStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                data.Serialize(binaryWriter);

                var request = requestPool.Rent();
                request.Data = memoryStream.ToArray();
                request.Command = officialCommand;

                package.Payload = Serializer.Serialize(request);
                package.PackageFlags = PackageFlags.Notification;
                client.SendPackageAndRelease(package);
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