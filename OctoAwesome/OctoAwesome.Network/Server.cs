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
        public bool IsRunning { get; private set; }

        public List<Client> Clients { get; set; }

        public event EventHandler<Client> OnClientConnected;

        private TcpListener tcpListener;


        public Server(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            tcpListener.Start();
            IsRunning = true;
            WorkingLoop();
        }

        public void Stop()
        {
            IsRunning = false;
            tcpListener.Stop();
        }

        private async void WorkingLoop()
        {
            while (IsRunning)
            {
                var client = await tcpListener.AcceptTcpClientAsync();
                await OnClientConnect(client);
            }
        }

        private async Task OnClientConnect(TcpClient tcpClient)
        {
            await Task.Yield();
            var client = new Client(tcpClient);
            Clients.Add(client);
            OnClientConnected?.Invoke(this, client);
        }
    }
}
