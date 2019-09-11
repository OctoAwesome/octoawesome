using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IObserver<Package>, INotificationObserver
    {
        private readonly Client client;
        private readonly IUpdateHub updateHub;
        private readonly IDisposable hubSubscription;
        private readonly IDisposable clientSubscription;
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IPool<ChunkNotification> chunkNotificationPool;
        private readonly IPool<Package> packagePool;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub)
        {
            this.client = client;
            this.updateHub = updateHub;

            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            chunkNotificationPool = TypeContainer.Get<IPool<ChunkNotification>>();
            packagePool = TypeContainer.Get<IPool<Package>>();

            hubSubscription = updateHub.Subscribe(this, DefaultChannels.Network);
            clientSubscription = client.Subscribe(this);
        }

        public void OnNext(Package package)
        {
            switch (package.OfficialCommand)
            {
                case OfficialCommand.EntityNotification:
                    var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, package.Payload);
                    updateHub.Push(entityNotification, DefaultChannels.Simulation);
                    entityNotification.Release();
                    break;
                case OfficialCommand.ChunkNotification:
                    var chunkNotification = Serializer.DeserializePoolElement(chunkNotificationPool, package.Payload);
                    updateHub.Push(chunkNotification, DefaultChannels.Chunk);
                    chunkNotification.Release();
                    break;
                default:
                    break;
            }
        }

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
                case ChunkNotification chunkNotification:
                    command = (ushort)OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(chunkNotification);
                    break;
                default:
                    return;
            }
            var package = packagePool.Get();
            package.Command = command;
            package.Payload = payload;
            client.SendPackageAndRelase(package);
        }

        public void OnError(Exception error)
            => throw error;

        public void OnCompleted()
            => clientSubscription.Dispose();
    }
}