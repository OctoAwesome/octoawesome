using System;
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
        public List<Client> Clients { get; set; }

        private TcpListener tcpListener;


        public Server()
        {
            tcpListener = new TcpListener(IPAddress.Any, 44444); //TODO: variable port?
        }

        public void Start()
        {
            tcpListener.Start();
            var socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += OnClientConnect;
            tcpListener.Server.AcceptAsync(socketArgs);

            //tcpListener.BeginAcceptTcpClient(OnClientConnect, null);
        }

        private void OnClientConnect(object sender, SocketAsyncEventArgs e)
        {
            var client = e.ConnectSocket;
            tcpListener.Server.AcceptAsync(e);
            
            //TODO Handshake aka Registration and authentication ????
            Clients.Add(new Client(e));

        }

        public void Stop()
        {
            tcpListener.Stop();
        }
    }
}
