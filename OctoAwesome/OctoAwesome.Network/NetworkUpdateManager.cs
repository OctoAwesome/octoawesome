using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IObserver<Package>
    {
        private readonly Client client;
        private readonly IUpdateHub updateHub;
        private readonly IDisposable subscription;
        private readonly IDefinitionManager definitionManager;

        public NetworkUpdateManager(Client client, IUpdateHub updateHub, IDefinitionManager manager)
        {
            this.client = client;
            this.updateHub = updateHub;
            subscription = client.Subscribe(this);
            definitionManager = manager;
        }

        public void OnNext(Package package)
        {
            switch (package.Command)
            {
                case (ushort)OfficialCommand.NewEntity:
                    var remoteEntity = Serializer.Deserialize<RemoteEntity>(package.Payload, definitionManager);
                    updateHub.Push(new EntityNotification()
                    {
                        Entity = remoteEntity,
                        Type = EntityNotification.ActionType.Add
                    });
                    break;
                case (ushort)OfficialCommand.RemoveEntity:
                    break;
                default:
                    break;
            }

        }

        public void OnError(Exception error)
            => throw error;

        public void OnCompleted()
            => subscription.Dispose();
    }
}