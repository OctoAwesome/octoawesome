using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace OctoAwesome.Network
{
    /// <summary>
    /// OctoAwesome network client implementation.
    /// </summary>
    public class Client : BaseClient
    {
        public Client(string host, ushort port) : base(new TcpClient())
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault();

            TcpClient.BeginConnect(address, port, OnConnected, null);
        }

        private void OnConnected(IAsyncResult ar)
        {
            TcpClient.EndConnect(ar);
            Start();
        }
    }
}