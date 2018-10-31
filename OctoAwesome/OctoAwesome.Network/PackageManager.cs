using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace OctoAwesome.Network
{
    public class PackageManager : ObserverBase<OctoNetworkEventArgs>
    {
        public List<BaseClient> ConnectedClients { get; set; }
        private Dictionary<BaseClient, Package> packages;
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;
        private readonly byte[] receiveBuffer;

        public PackageManager()
        {
            packages = new Dictionary<BaseClient, Package>();
            ConnectedClients = new List<BaseClient>();
        }

        public void AddConnectedClient(BaseClient client) => client.Subscribe(this);

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(OctoNetworkEventArgs e)
        {
            var baseClient = e.Client;

            byte[] bytes;
            bytes = new byte[e.DataCount];

            if (!packages.TryGetValue(baseClient, out Package package))
            {
                package = new Package();
                packages.Add(baseClient, package);

                int current = 0;

                current += e.NetworkStream.Read(bytes, current, Package.HEAD_LENGTH - current);

                if (current != Package.HEAD_LENGTH)
                {
                    Console.WriteLine($"Package was not complete, only got: {current} bytes");
                    packages.Remove(baseClient);
                    return;
                }


                package.TryDeserializeHeader(bytes);
                e.DataCount -= Package.HEAD_LENGTH;
            }

            if (e.DataCount > 0)
                e.NetworkStream.Read(bytes, 0, e.DataCount);
            var count = package.DeserializePayload(bytes, 0, e.DataCount);

            if (package.IsComplete)
            {
                packages.Remove(baseClient);
                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });

                if (e.DataCount - count > 0)
                    ClientDataAvailable(new OctoNetworkEventArgs() { Client = baseClient, DataCount = e.DataCount - count, NetworkStream = e.NetworkStream });
            }
        }

        protected override void OnNextCore(OctoNetworkEventArgs args)
        {
            ClientDataAvailable(args);
        }

        protected override void OnErrorCore(Exception error) => throw new NotImplementedException();
        protected override void OnCompletedCore() => throw new NotImplementedException();
    }
}
