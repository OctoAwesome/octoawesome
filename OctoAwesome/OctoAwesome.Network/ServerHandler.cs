using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class ServerHandler
    {
        public Server Server { get; }
        public SimulationManager SimulationManager { get; set; }
        //TODO: Should use a base class or interface
        public ServerHandler(Server server)
        {
            Server = server;
            Server.OnClientConnected += ServerOnClientConnected;
            SimulationManager = new SimulationManager(new Settings());
        }
        
        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            //e.OnMessageRecived += OnMessageRecive;
        }

        private int OnMessageRecive(object sender, (byte[] Data,int Offset, int Count) eventArgs)
        {
            var client = (ConnectedClient)sender;
            return 0;
        }
    }
}
