using OctoAwesome.Caching;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Network.Request;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

using System;
using System.IO;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    /// <summary>
    /// OctoAwesome client implementation for handling connected clients in a <see cref="Server"/>.
    /// </summary>
    public sealed class ConnectedClient : BaseClient
    {
        private readonly IDisposable networkSubscription;

        /// <summary>
        /// Gets or sets the <see cref="Server"/> package subscription associated with this client.
        /// </summary>
        public IDisposable? ServerSubscription { get; set; }

        private readonly PackagePool packagePool;
        private readonly Pool<OfficialCommandDTO> requestPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedClient"/> class.
        /// </summary>
        /// <param name="socket">The low level base socket.</param>
        public ConnectedClient(TcpClient socket) : base(socket)
        {
            packagePool = TypeContainer.Get<PackagePool>();
            var updateHub = TypeContainer.Get<IUpdateHub>();
            networkSubscription = updateHub.ListenOn(DefaultChannels.Network).Subscribe(OnNext, OnError);
            requestPool = new();
        }


        private void OnError(Exception error)
        {
            TcpClient.Close();
            throw error;
        }

        private void OnNext(object value)
        {
            if (value is not Notification notification)
                return;

            if (notification.SenderId == Id)
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

                package.PackageFlags = PackageFlags.Notification;
                package.Payload = Serializer.Serialize(request);
                SendPackageAndRelease(package);
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            ServerSubscription?.Dispose();
            networkSubscription.Dispose();
        }
    }
}