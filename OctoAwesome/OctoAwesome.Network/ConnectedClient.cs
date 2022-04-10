﻿using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectedClient"/> class.
        /// </summary>
        /// <param name="socket">The low level base socket.</param>
        public ConnectedClient(Socket socket) : base(socket)
        {
            packagePool = TypeContainer.Get<PackagePool>();
            var updateHub = TypeContainer.Get<IUpdateHub>();
            networkSubscription = updateHub.ListenOn(DefaultChannels.Network).Subscribe(OnNext, OnError);
        }


        private void OnError(Exception error)
        {
            Socket.Close();
            throw error;
        }

        private void OnNext(Notification value)
        {
            if (value.SenderId == Id)
                return;

            OfficialCommand command;
            byte[] payload;
            switch (value)
            {
                case EntityNotification entityNotification:
                    command = OfficialCommand.EntityNotification;
                    payload = Serializer.Serialize(entityNotification);
                    break;

                case BlocksChangedNotification _:
                case BlockChangedNotification _:
                    command = OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize((SerializableNotification)value);
                    break;
                default:
                    return;
            }

            BuildAndSendPackage(payload, command);
        }

        private void BuildAndSendPackage(byte[] data, OfficialCommand officialCommand)
        {
            var package = packagePool.Rent();
            package.Payload = data;
            package.Command = (ushort)officialCommand;
            SendPackageAndRelease(package);
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