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

        private World world;

        private ServiceHost host;

        private List<Client> clients = new List<Client>();

        public Server()
        {

        }

        public void Open()
        {
            world = new World();

            string server = "localhost";
            int port = 8888;
            string name = "Octo";

            string address = string.Format("net.tcp://{0}:{1}/{2}", server, port, name);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            host = new ServiceHost(typeof(Client), new Uri(address));
            host.AddServiceEndpoint(typeof(IConnection), binding, address);
            host.Open();
        }

        public void Close()
        {
            if (host == null)
                return;

            // TODO: Alle Klienten schließen
            lock (clients)
            {
                foreach (var client in clients)
                {
                    try
                    {
                        client.Callback.Disconnect(string.Empty);
                    }
                    catch (Exception) { }

                    clients.Remove(client);
                    if (OnDeregister != null)
                        OnDeregister(client);
                }
            }

            host.Close();
        }

        internal void Join(Client client)
        {
            lock (clients)
            {
                clients.Add(client);
                if (OnRegister != null)
                    OnRegister(client);

                ActorHost actorHost = world.InjectPlayer(new Player()
                {
                    Position = new Coordinate(0, new Index3(100, 100, 300), new Microsoft.Xna.Framework.Vector3()),
                });
                client.SetActorHost(actorHost);
                actorHost.Initialize();
            }
        }

        internal void Leave(Client client)
        {
            lock (clients)
            {
                try
                {
                    client.Callback.Disconnect(string.Empty);
                }
                catch (Exception) { }

                clients.Remove(client);
                if (OnDeregister != null)
                    OnDeregister(client);
            }
        }

        public IEnumerable<Client> Clients
        {
            get
            {
                lock (clients)
                {
                    return clients;
                }
            }
        }

        public event RegisterDelegate OnRegister;

        public event RegisterDelegate OnDeregister;

        public delegate void RegisterDelegate(Client info);
    }
}
