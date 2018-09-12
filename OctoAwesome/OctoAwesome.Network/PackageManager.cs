using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class PackageManager
    {
        public List<BaseClient> ConnectedClients { get; set; }


        public void AddConnectedClient(BaseClient client)
        {
            client.BeginRecived(out OctoNetworkStream recivedStream);
            client.BeginSend(out OctoNetworkStream sendStream);
        }
    }
}
