using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
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

        public ActorProxy PlayerController { get;  private set; }

        public ClientComponent(OctoGame game) : base(game)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            DuplexChannelFactory<IConnection> factory = new DuplexChannelFactory<IConnection>(this, binding);
            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:8888/Octo");
            client = factory.CreateChannel(endpoint);

            try
            {
                connectionId = client.Connect("test");
            }
            catch (Exception ex)
            {
                return;
            }

            PlayerController = new ActorProxy(client);

            NetTcpBinding chunkBinding = new NetTcpBinding(SecurityMode.None);
            chunkBinding.TransferMode = TransferMode.Streamed;

            ChannelFactory<IChunkConnection> chunkFactory = new ChannelFactory<IChunkConnection>(chunkBinding);
            EndpointAddress chunkEndpoint = new EndpointAddress("net.tcp://localhost:8888/Chunks");
            chunkClient = chunkFactory.CreateChannel(chunkEndpoint);

            using (Stream stream = chunkClient.GetChunk(0, 1, 1, 1))
            {
                ChunkSerializer serializer = new ChunkSerializer();
                IChunk chunk = serializer.Deserialize(stream, new PlanetIndex3(0, 1, 1, 1));
            }
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
    }
}
