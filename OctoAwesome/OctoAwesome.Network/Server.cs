using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    class Server
    {
        public event EventHandler<ConnectedClient> OnClientConnected;
        public SimulationManager SimulationManager { get; set; }

        private Socket socket;
        private List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lockObj = new object();

            SimulationManager = new SimulationManager(new Settings());
        }

        public void Start(IPAddress address, int port)
        {
            connectedClients = new List<ConnectedClient>();
            socket.Bind(new IPEndPoint(address, port));
            socket.Listen(1024);
            socket.BeginAccept(OnClientAccepted, null);
        }
        public void Start(string host, int port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == socket.AddressFamily);

            Start(address, port);
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            var tmpSocket = socket.EndAccept(ar);
            socket.BeginAccept(OnClientAccepted, null);
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            client.Start();

            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);
        }
    }
}
