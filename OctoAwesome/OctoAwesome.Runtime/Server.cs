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

        private ServiceHost playerHost;

        private ServiceHost chunkHost;

        public bool IsRunning { get; private set; }

        private List<Client> clients = new List<Client>();

        private Dictionary<Client, ClientInfo> clientInfos = new Dictionary<Client, ClientInfo>();

        public Server()
        {

        }

        public void Open()
        {
            world = new World();

            string server = "localhost";
            int port = 8888;
            string playerName = "Octo";
            string chunkName = "Chunks";

            string playerAddress = string.Format("net.tcp://{0}:{1}/{2}", server, port, playerName);
            string chunkAddress = string.Format("net.tcp://{0}:{1}/{2}", server, port, chunkName);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            playerHost = new ServiceHost(typeof(Client), new Uri(playerAddress));
            playerHost.AddServiceEndpoint(typeof(IConnection), binding, playerAddress);
            playerHost.Open();

            NetTcpBinding chunkBinding = new NetTcpBinding(SecurityMode.None);
            chunkBinding.TransferMode = TransferMode.Streamed;
            chunkBinding.MaxReceivedMessageSize = int.MaxValue;
            chunkBinding.MaxBufferSize = int.MaxValue;

            chunkHost = new ServiceHost(typeof(ChunkConnection), new Uri(chunkAddress));
            chunkHost.AddServiceEndpoint(typeof(IChunkConnection), chunkBinding, chunkAddress);
            chunkHost.Open();

            IsRunning = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            if (playerHost == null)
                return;

            // TODO: Alle Klienten schließen
            lock (clients)
            {
                //Safe Close Clients
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        clients[i].Callback.Disconnect("Server closed");
                    }
                    catch (Exception) { }

                    if (OnLeave != null)
                        OnLeave(clients[i]);
                }

                clients.Clear();
            }

            playerHost.Close();
            chunkHost.Close();

            IsRunning = false;
        }

        public void Kick(Guid client)
        {
            var c = clients.FirstOrDefault(x => x.ConnectionId == client);
            if (c != null)
                c.Disconnect("Kicked");
        }

        internal void Join(Client client)
        {
            lock (clients)
            {
                ClientInfo info = new ClientInfo()
                {
                    Id = client.ConnectionId,
                    Name = client.Playername
                };
                clientInfos.Add(client, info);

                // Alle anderen Clients informieren
                foreach (var c in clients)
                {
                    try
                    {
                        c.Callback.SendPlayerJoin(info);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Disconnect
                    }
                }

                // Vollständige Client-Liste an neuen Client
                try
                {
                    // client.Callback.SendPlayerList(clientInfos.Values);
                }
                catch (Exception ex)
                {
                    return;
                }

                clients.Add(client);
                if (OnJoin != null)
                    OnJoin(client);

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
                clientInfos.Remove(client);
                if (OnLeave != null)
                    OnLeave(client);

                // ActorHost entfernen
                world.RemovePlayer(client.ActorHost);

                // Alle anderen Clients informieren
                foreach (var c in clients)
                {
                    try
                    {
                        c.Callback.SendPlayerLeave(client.ConnectionId);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Disconnect
                    }
                }
            }
        }

        public IEnumerable<ClientInfo> Clients
        {
            get
            {
                lock (clients)
                {
                    return clientInfos.Values.ToArray();
                }
            }
        }

        public event RegisterDelegate OnJoin;

        public event RegisterDelegate OnLeave;

        public delegate void RegisterDelegate(Client info);
    }
}
