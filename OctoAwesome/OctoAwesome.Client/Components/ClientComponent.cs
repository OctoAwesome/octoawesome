using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal class ClientComponent : GameComponent, IConnectionCallback
    {
        private Guid connectionId;

        private IConnection client;

        private IChunkConnection chunkClient;

        private List<ClientInfo> otherPlayers = new List<ClientInfo>();

        public ActorProxy PlayerController { get; private set; }

        public ClientComponent(OctoGame game) : base(game)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            DuplexChannelFactory<IConnection> factory = new DuplexChannelFactory<IConnection>(this, binding);
            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:8888/Octo");
            client = factory.CreateChannel(endpoint);

            try
            {
                var result = client.Connect(ConfigurationManager.AppSettings["Playername"]);
                connectionId = result.Id;
                otherPlayers.Clear();
                otherPlayers.AddRange(result.OtherClients);
            }
            catch (Exception ex)
            {
                return;
            }

            PlayerController = new ActorProxy(client);

            NetTcpBinding chunkBinding = new NetTcpBinding(SecurityMode.None);
            chunkBinding.TransferMode = TransferMode.Streamed;
            chunkBinding.MaxReceivedMessageSize = int.MaxValue;
            chunkBinding.MaxBufferSize = int.MaxValue;

            ChannelFactory<IChunkConnection> chunkFactory = new ChannelFactory<IChunkConnection>(chunkBinding);
            EndpointAddress chunkEndpoint = new EndpointAddress("net.tcp://localhost:8888/Chunks");
            chunkClient = chunkFactory.CreateChannel(chunkEndpoint);

            ResourceManager.Instance.GlobalChunkCache = new GlobalChunkCache((position) =>
            {
                using (Stream stream = chunkClient.SubscribeChunk(connectionId, position))
                {
                    ChunkSerializer serializer = new ChunkSerializer();
                    return serializer.Deserialize(stream, position);
                }
            }, (position, chunk) =>
            {
                client.UnsubscribeChunk(position);
            });
        }

        public override void Initialize()
        {
            base.Initialize();


        }

        protected override void Dispose(bool disposing)
        {
            if (client != null)
            {
                try
                {
                    client.Disconnect(string.Empty);
                }
                catch (Exception) { }
            }

            base.Dispose(disposing);
        }

        public void Disconnect(string reason)
        {
            if (OnDisconnect != null)
                OnDisconnect.Invoke(reason);
        }

        public void SetPosition(int planet, Index3 globalPosition, Vector3 blockPosition)
        {
            PlayerController.Position = new Coordinate(planet, globalPosition, blockPosition);
        }

        public void SetAngle(float value)
        {
            PlayerController.Angle = value;
        }

        public void SetFlyMode(bool value)
        {
            PlayerController.FlyMode = value;
        }

        public void SetHead(Vector2 value)
        {
            PlayerController.Head = value;
        }

        public void SetHeight(float value)
        {
            PlayerController.Height = value;
        }

        public void SetMove(Vector2 value)
        {
            PlayerController.Move = value;
        }

        public void SetOnGround(bool value)
        {
            PlayerController.OnGround = value;
        }

        public void SetRadius(float value)
        {
            PlayerController.Radius = value;
        }

        public void SetTilt(float value)
        {
            PlayerController.Tilt = value;
        }

        public void SendPlayerJoin(ClientInfo client)
        {
            otherPlayers.Add(client);
        }

        public void SendPlayerLeave(Guid client)
        {
            var c = otherPlayers.FirstOrDefault(p => p.Id == client);
            if (c != null)
                otherPlayers.Remove(c);
        }

        public void SendBlockRemove(int planet, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ)
        {
            throw new NotImplementedException();
        }

        public void SendBlockInsert(int planet, int chunkX, int chunkY, int chunkZ, int blockX, int blockY, int blockZ, string fullName, int metaData)
        {
            throw new NotImplementedException();
        }

        public void SendEntityInsert(int planet, int chunkX, int chunkY, int chunkZ, Guid id, string fullName, byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendEntityRemove(Guid id)
        {
            throw new NotImplementedException();
        }

        public void SendEntityMove(Guid id, int planet, int chunkX, int chunkY, int chunkZ)
        {
            throw new NotImplementedException();
        }

        public void SendEntityUpdate(Guid id, byte[] data)
        {
            throw new NotImplementedException();
        }

        public delegate void DisconnectDelegate(string reason);

        public event DisconnectDelegate OnDisconnect;
    }
}
