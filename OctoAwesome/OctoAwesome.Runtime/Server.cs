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

        private Dictionary<PlanetIndex3, List<Client>> subscriptions = new Dictionary<PlanetIndex3, List<Client>>();

        private Dictionary<Client, ClientInfo> clientInfos = new Dictionary<Client, ClientInfo>();

        public Server()
        {

        }

        public void Open()
        {
            world = new World();

            string server = "localhost";
            int port1 = 8888;
            int port2 = 8889;
            string playerName = "Octo";
            string chunkName = "Chunks";

            string playerAddress = string.Format("net.tcp://{0}:{1}/{2}", server, port1, playerName);
            string chunkAddress = string.Format("net.tcp://{0}:{1}/{2}", server, port2, chunkName);

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
                        HandleException(ex, c);
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

                // Alle Chunks deabonieren
                foreach (var chunk in client.SubscripedChunks)
                    UnsubscribeChunk(client.ConnectionId, chunk);
                client.SubscripedChunks.Clear();

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
                        HandleException(ex, c);
                    }
                }
            }
        }

        internal void SubscibeChunk(Guid clientId, PlanetIndex3 chunkIndex)
        {
            lock (subscriptions)
            {
                var client = clients.SingleOrDefault(c => c.ConnectionId == clientId);
                if (client == null) return;

                if (!subscriptions.ContainsKey(chunkIndex))
                    subscriptions.Add(chunkIndex, new List<Client>());
                subscriptions[chunkIndex].Add(client);

                IChunk chunk = ResourceManager.Instance.GlobalChunkCache.Subscribe(chunkIndex, false);
                foreach (var entity in chunk.Entities.ToArray())
                {
                    try
                    {
                        client.Callback.SendEntityInsert(chunkIndex, entity.Id, entity.GetType().Assembly.GetName().Name, entity.GetType().FullName, entity.GetData());
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex, client);
                    }
                }
            }
        }

        internal void UnsubscribeChunk(Guid clientId, PlanetIndex3 chunkIndex)
        {
            lock (subscriptions)
            {
                var client = clients.SingleOrDefault(c => c.ConnectionId == clientId);
                if (client == null) return;

                if (subscriptions.ContainsKey(chunkIndex))
                {
                    subscriptions[chunkIndex].Remove(client);
                    if (subscriptions[chunkIndex].Count == 0)
                        subscriptions.Remove(chunkIndex);
                }
            }
        }

        internal void InsertEntity(Entity entity, IChunk destination)
        {
            if (destination == null)
                return;

            PlanetIndex3 index = new PlanetIndex3(destination.Planet, destination.Index);
            lock (subscriptions)
            {
                List<Client> clients;
                if (subscriptions.TryGetValue(index, out clients))
                {
                    byte[] data = entity.GetData();
                    foreach (var client in clients)
                    {
                        try
                        {
                            client.Callback.SendEntityInsert(index, entity.Id, entity.GetType().Assembly.GetName().Name, entity.GetType().FullName, data);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        internal void RemoveEntity(Entity entity, IChunk departure)
        {
            if (departure == null)
                return;

            PlanetIndex3 index = new PlanetIndex3(departure.Planet, departure.Index);
            lock (subscriptions)
            {
                List<Client> clients;
                if (subscriptions.TryGetValue(index, out clients))
                {
                    foreach (var client in clients)
                    {
                        try
                        {
                            client.Callback.SendEntityRemove(entity.Id);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        internal void MoveEntity(Entity entity, IChunk departure, IChunk destination)
        {
            if (departure == null && destination == null)
                return;

            if (departure == null)
            {
                InsertEntity(entity, destination);
                return;
            }

            if (destination == null)
            {
                RemoveEntity(entity, departure);
                return;
            }

            PlanetIndex3 destinationIndex = new PlanetIndex3(destination.Planet, destination.Index);
            PlanetIndex3 departureIndex = new PlanetIndex3(departure.Planet, departure.Index);

            lock (subscriptions)
            {
                List<Client> departureList;
                if (!subscriptions.TryGetValue(departureIndex, out departureList))
                    departureList = new List<Client>();

                List<Client> destinationList;
                if (!subscriptions.TryGetValue(destinationIndex, out destinationList))
                    destinationList = new List<Client>();

                var clients = departureList.Union(destinationList);
                byte[] data = entity.GetData();
                foreach (var client in clients)
                {
                    if (!departureList.Contains(client))
                    {
                        try
                        {
                            client.Callback.SendEntityInsert(destinationIndex, entity.Id, entity.GetType().Assembly.GetName().Name, entity.GetType().FullName, data);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                    else if (!destinationList.Contains(client))
                    {
                        try
                        {
                            client.Callback.SendEntityRemove(entity.Id);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                    else
                    {
                        try
                        {
                            client.Callback.SendEntityMove(entity.Id, destinationIndex);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        internal void UpdateEntity(Entity entity)
        {
            PlanetIndex3 chunkIndex = new PlanetIndex3(entity.Position.Planet, entity.Position.ChunkIndex);
            lock (subscriptions)
            {
                List<Client> clients;
                if (subscriptions.TryGetValue(chunkIndex, out clients))
                {
                    byte[] data = entity.GetData();
                    foreach (var client in clients)
                    {
                        try
                        {
                            client.Callback.SendEntityUpdate(entity.Id, data);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        internal void RemoveBlock(int planet, Index3 index)
        {
            PlanetIndex3 chunkIndex = new PlanetIndex3(planet, new Index3(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ));
            Index3 blockIndex = new Index3(index.X & ((1 << Chunk.LimitX) - 1), index.Y & ((1 << Chunk.LimitY) - 1), index.Z & ((1 << Chunk.LimitZ) - 1));

            lock (subscriptions)
            {
                List<Client> clients;
                if (subscriptions.TryGetValue(chunkIndex, out clients))
                {
                    foreach (var client in clients)
                    {
                        try
                        {
                            client.Callback.SendBlockRemove(chunkIndex, blockIndex);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        internal void AddBlock(int planet, Index3 index, IBlockDefinition blockDefinition, int meta)
        {
            PlanetIndex3 chunkIndex = new PlanetIndex3(planet, new Index3(index.X >> Chunk.LimitX, index.Y >> Chunk.LimitY, index.Z >> Chunk.LimitZ));
            Index3 blockIndex = new Index3(index.X & ((1 << Chunk.LimitX) - 1), index.Y & ((1 << Chunk.LimitY) - 1), index.Z & ((1 << Chunk.LimitZ) - 1));

            lock (subscriptions)
            {
                List<Client> clients;
                if (subscriptions.TryGetValue(chunkIndex, out clients))
                {
                    foreach (var client in clients)
                    {
                        try
                        {
                            client.Callback.SendBlockInsert(chunkIndex, blockIndex, blockDefinition.GetType().FullName, meta);
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex, client);
                        }
                    }
                }
            }
        }

        private void HandleException(Exception ex, Client client)
        {
            // TODO: Handle Exception!
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
