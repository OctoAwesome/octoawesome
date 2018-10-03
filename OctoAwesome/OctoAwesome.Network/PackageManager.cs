using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class PackageManager
    {
        public List<BaseClient> ConnectedClients { get; set; }
        private Dictionary<BaseClient, Package> packages;
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;


        public PackageManager()
        {
            packages = new Dictionary<BaseClient, Package>();
            ConnectedClients = new List<BaseClient>();
        }

        public void AddConnectedClient(BaseClient client)
        {
            client.DataAvailable += ClientDataAvailable;
        }

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        private void ClientDataAvailable(object sender, OctoNetworkEventArgs e)
        {
            Package package;
            var baseClient = (BaseClient)sender; 
            byte[] bytes = new byte[e.DataCount];
            if (!packages.TryGetValue(baseClient, out package))
            {
                package = new Package();
                packages.Add(baseClient, package);
                if (e.DataCount >= Package.HEAD_LENGTH)
                {
                    e.NetworkStream.Read(bytes, 0, Package.HEAD_LENGTH);
                    package.TryDeserializeHeader(bytes);
                    e.DataCount -= Package.HEAD_LENGTH;
                }
            }

            e.NetworkStream.Read(bytes, 0, e.DataCount);
            package.DeserializePayload(bytes, 0, e.DataCount);

            if (package.IsComplete)
            {
                packages.Remove(baseClient);
                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });
            }
        }
    }
}
