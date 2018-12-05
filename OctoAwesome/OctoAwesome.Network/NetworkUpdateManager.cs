using OctoAwesome.Network;
using System;

namespace OctoAwesome.Network
{
    public class NetworkUpdateManager : IObserver<Package>
    {
        private readonly Client client;
        private readonly IDisposable subscription;

        public NetworkUpdateManager(Client client)
        {
            this.client = client;
            subscription = client.Subscribe(this);
        }

        public void OnNext(Package package)
        {
            
        }

        public void OnError(Exception error)
            => throw error;

        public void OnCompleted()
            => subscription.Dispose();
    }
}