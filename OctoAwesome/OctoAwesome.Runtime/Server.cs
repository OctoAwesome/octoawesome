using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    public sealed class Server
    {
        #region Singleton

        private static Server instance;

        public static Server Instance
        {
            get
            {
                if (instance == null)
                    instance = new Server();
                return instance;
            }
        }

        #endregion

        ServiceHost host;

        private List<Client> clients = new List<Client>();

        public Server()
        {

        }

        public void Open()
        {
            string server = "localhost";
            int port = 8888;
            string name = "Octo";

            string address = string.Format("net.tcp://{0}:{1}/{2}", server, port, name);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            host = new ServiceHost(typeof(Client), new Uri(address));
            host.AddServiceEndpoint(typeof(IClient), binding, address);
            host.Open();
        }

        public void Close()
        {
            // TODO: Alle Klienten schließen

            host.Close();
        }

        internal void Register(Client client)
        {
            lock (clients)
            {
                clients.Add(client);
            }
        }

        internal void Deregister(Client client)
        {
            lock (clients)
            {
                clients.Remove(client);
            }
        }

        public IEnumerable<string> Clients
        {
            get
            {
                lock (clients)
                {
                    return clients.Select(c => c.Playername);
                }
            }
        }
    }
}
