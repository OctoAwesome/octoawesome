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

        private TcpListener tcpListener = default!;
        private readonly List<ConnectedClient> connectedClients;
        private readonly object lockObj;

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        public Server()
        {

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

            tcpListener = new TcpListener(endpoints.First());
            tcpListener.Server.DualMode = true;
            tcpListener.Server.NoDelay = true;

            foreach (var endpoint in endpoints.Skip(1))
                tcpListener.Server.Bind(endpoint);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(OnClientAccepted, tcpListener);

        }
        /// <summary>
        /// Starts listening on the specified host and port.
        /// </summary>
        /// <param name="host">The host to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public void Start(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).Where(
                a => a.AddressFamily == AddressFamily.InterNetwork || a.AddressFamily == AddressFamily.InterNetworkV6);

            Start(address.Select(a => new IPEndPoint(a, port)).ToArray());
        }

        public void Stop()
        {
            ipv4Socket.Close();
            ipv6Socket.Close();
            foreach (var item in connectedClients)
            {
                item.Stop();
            }
            connectedClients.Clear();
        }

        private void OnClientAccepted(IAsyncResult ar)
        {
            Debug.Assert(ar.AsyncState != null, "ar.AsyncState != null");
            var listener = (TcpListener)ar.AsyncState;

            var tmpSocket = listener.EndAcceptTcpClient(ar);

            tmpSocket.NoDelay = true;

            var client = new ConnectedClient(tmpSocket);
            client.Start();

            OnClientConnected?.Invoke(this, client);

            lock (lockObj)
                connectedClients.Add(client);

            listener.BeginAcceptTcpClient(OnClientAccepted, listener);
        }
    }
}
