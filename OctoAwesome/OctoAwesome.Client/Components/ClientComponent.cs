using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal class ClientComponent : GameComponent, IClientCallback
    {
        public ClientComponent(OctoGame game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            DuplexChannelFactory<IClient> factory = new DuplexChannelFactory<IClient>(this, binding);
            EndpointAddress endpoint = new EndpointAddress("net.tcp://localhost:8888/Octo");
            IClient client = factory.CreateChannel(endpoint);

            try
            {
                client.Connect("test");
            }
            catch (Exception ex)
            {

            }
        }

        public void Relocation(int x, int y, int z)
        {
            Console.WriteLine(string.Format("{0}/{1}/{2}", x, y, z));
        }
    }
}
