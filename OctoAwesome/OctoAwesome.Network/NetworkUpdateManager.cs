using OctoAwesome.Network;
using OctoAwesome.Notifications;
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
        private readonly IDefinitionManager definitionManager;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub, IDefinitionManager manager)
        {
            this.client = client;
            this.updateHub = updateHub;
            hubSubscription = updateHub.Subscribe(this, DefaultChannels.Network);
            clientSubscription = client.Subscribe(this);
            definitionManager = manager;
        }

        public void OnNext(Package package)
        {
            switch (package.Command)
            {
                case (ushort)OfficialCommand.EntityNotification:
                    var entityNotification = Serializer.Deserialize<EntityNotification>(package.Payload, definitionManager);
                    updateHub.Push(entityNotification, DefaultChannels.Simulation);
                    break;
                case (ushort)OfficialCommand.ChunkNotification:
                    var chunkNotification = Serializer.Deserialize<ChunkNotification>(package.Payload, definitionManager);
                    updateHub.Push(chunkNotification, DefaultChannels.Chunk);
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
                    payload = Serializer.Serialize(entityNotification, definitionManager);
                    break;
                case ChunkNotification chunkNotification:
                    command = (ushort)OfficialCommand.ChunkNotification;
                    payload = Serializer.Serialize(chunkNotification, definitionManager);
                    break;
                default:
                    return;
            }

            client.SendPackage(new Package(command, 0)
            {
                Payload = payload
            });
        }

        public void OnError(Exception error)
            => throw error;

        public void OnCompleted()
            => clientSubscription.Dispose();
    }
}