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
            networkSubscription = updateHub.ListenOnNetwork().Subscribe(OnNext, OnError);
            requestPool = new();
        }

        private void OnError(Exception error)
        {
            TcpClient.Close();
            throw error;
        }

        private void OnNext(PushInfo value)
        {
            if (value.Notification is not SerializableNotification notification)
            {
                SendGeneric(value);
                return;
            }

            if (notification.SenderId == Id)
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
                SendPackageAndRelease(package);
            }
        }


        /// <inheritdoc />
        public override void Dispose()
        {
            base.Dispose();
            networkSubscription?.Dispose();
            ServerSubscription?.Dispose();
        }
    }
}