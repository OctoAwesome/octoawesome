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
    public class Server //TODO: Should use a base class or interface
    {
        public event EventHandler<ConnectedClient> OnClientConnected;

        private Socket socket;
        private List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            lockObj = new object();

        }

        public void Start(IPAddress address, ushort port)
        {
            connectedClients = new List<ConnectedClient>();
            socket.Bind(new IPEndPoint(address, port));
            socket.Listen(1024);
            socket.BeginAccept(OnClientAccepted, null);
        }
        public void Start(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == socket.AddressFamily);

            Start(address, port);
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            var tmpSocket = socket.EndAccept(ar);
            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            client.Start();

            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);
            socket.BeginAccept(OnClientAccepted, null);
        }
    }
}
