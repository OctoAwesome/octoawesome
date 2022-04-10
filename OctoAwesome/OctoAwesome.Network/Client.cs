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
        /// <summary>
        /// Connect to an OctoAwesome server on a specific port.
        /// </summary>
        /// <param name="host">The host ip or address to connect to.</param>
        /// <param name="port">The port to connect over.</param>
        /// <exception cref="ArgumentException">Thrown when the host address could not be resolved.</exception>
        public void Connect(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault();
            if (address == default)
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