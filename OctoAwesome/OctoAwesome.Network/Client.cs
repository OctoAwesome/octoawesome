using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace OctoAwesome.Network
{

    public class Client : BaseClient
    {
        public void Connect(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault();
            if(address == default)
            {
                throw new ArgumentException(nameof(host));
            }
            Socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket.BeginConnect(new IPEndPoint(address, port), OnConnected, null);
        }

        private void OnConnected(IAsyncResult ar)
        {
            Socket.EndConnect(ar);

            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }
        }
    }
}