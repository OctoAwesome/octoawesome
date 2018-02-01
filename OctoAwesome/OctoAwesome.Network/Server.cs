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
        public ConcurrentBag<Client> Clients { get; set; }

        public event EventHandler<Client> OnClientConnected;

        private Socket socket;

        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Clients = new ConcurrentBag<Client>();
        }

        public void Start(int port)
        {
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(128);
            socket.BeginAccept(OnClientConnect, null);
        }

        private void OnClientConnect(IAsyncResult ar)
        {
            var tmpSocket = socket.EndAccept(ar);
            socket.BeginAccept(OnClientConnect, null);

            tmpSocket.NoDelay = true;

            var client = new Client(tmpSocket);
            client.Listening();


            Clients.Add(client);
            
            OnClientConnected?.Invoke(this, client);
        }

        public void Stop()
        {
            socket.Disconnect(true);
        }
        

       
    }
}
