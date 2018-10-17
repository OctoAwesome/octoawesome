using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Network
{
    public class PackageManager
    {
        public List<BaseClient> ConnectedClients { get; set; }
        private Dictionary<BaseClient, Package> packages;
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;
        private byte[] receiveBuffer;

        public PackageManager()
        {
            packages = new Dictionary<BaseClient, Package>();
            ConnectedClients = new List<BaseClient>();
            receiveBuffer = new byte[0];
        }

        public void AddConnectedClient(BaseClient client) => client.DataAvailable += ClientDataAvailable;

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(object sender, OctoNetworkEventArgs e)
        {
            var baseClient = (BaseClient)sender;
            byte[] bytes;

            if (receiveBuffer.Length > 0)
            {
                bytes = receiveBuffer.Concat(new byte[e.DataCount]).ToArray();
                receiveBuffer = new byte[0];
            }
            else
            {
                bytes = new byte[e.DataCount];
            }

            if (!packages.TryGetValue(baseClient, out Package package))
            {
                package = new Package();
                packages.Add(baseClient, package);
                if (e.DataCount >= Package.HEAD_LENGTH)
                {
                    e.NetworkStream.Read(bytes, 0, Package.HEAD_LENGTH);
                    package.TryDeserializeHeader(bytes);
                    if (package.Command == 0)
                        ;
                    e.DataCount -= Package.HEAD_LENGTH;
                }
                else
                {
                    receiveBuffer = new byte[e.DataCount];
                    e.NetworkStream.Read(receiveBuffer, 0, e.DataCount);
                    packages.Remove(baseClient);
                    return;
                }
            }

            e.NetworkStream.Read(bytes, 0, e.DataCount);
            var count = package.DeserializePayload(bytes, 0, e.DataCount);

            if (package.IsComplete)
            {
                packages.Remove(baseClient);
                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });

                if (e.DataCount - count > 0)
                    ClientDataAvailable(sender, new OctoNetworkEventArgs() { DataCount = e.DataCount - count, NetworkStream = e.NetworkStream });
            }
        }
    }
}
