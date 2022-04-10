using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    /// <summary>
    /// OctoAwesome server implementation.
    /// </summary>
    public class Server //TODO: Should use a base class or interface
    {
        /// <summary>
        /// Called when a new client has connected to the server.
        /// </summary>
        public event EventHandler<ConnectedClient>? OnClientConnected;

        private readonly Socket ipv4Socket;
        private readonly Socket ipv6Socket;
        private readonly List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        public Server()
        {
            ipv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            connectedClients = new List<ConnectedClient>();
            lockObj = new object();

        }

        /// <summary>
        /// Starts to listen for connections on the given endpoints.
        /// </summary>
        /// <param name="endpoints">The endpoints to listen on.</param>
        public void Start(params IPEndPoint[] endpoints)
        {
            connectedClients.Clear();

            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                foreach (var endpoint in endpoints)
                    if (endpoint.AddressFamily == AddressFamily.InterNetwork)
                        ipv4Socket.Bind(endpoint);

                ipv4Socket.Listen(1024);
                ipv4Socket.BeginAccept(OnClientAccepted, ipv4Socket);
            }
            if (endpoints.Any(x => x.AddressFamily == AddressFamily.InterNetworkV6))
            {
                foreach (var endpoint in endpoints.Where(e => e.AddressFamily == AddressFamily.InterNetworkV6))
                    ipv6Socket.Bind(endpoint);

                ipv6Socket.Listen(1024);
                ipv6Socket.BeginAccept(OnClientAccepted, ipv6Socket);
            }
        }
        /// <summary>
        /// Starts listening on the specified host and port.
        /// </summary>
        /// <param name="host">The host to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public void Start(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).Where(
                a => a.AddressFamily == ipv4Socket.AddressFamily || a.AddressFamily == ipv6Socket.AddressFamily);

            Start(address.Select(a => new IPEndPoint(a, port)).ToArray());
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            Debug.Assert(ar.AsyncState != null, "ar.AsyncState != null");
            var socket = (Socket)ar.AsyncState;

            var tmpSocket = socket!.EndAccept(ar);

            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            client.Start();

            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);

            socket.BeginAccept(OnClientAccepted, socket);
        }
    }
}
