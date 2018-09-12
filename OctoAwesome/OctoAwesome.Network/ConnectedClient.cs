using System;
using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OctoAwesome.Network
{
    public class ConnectedClient : BaseClient
    {
        private static int received;
        

        public ConnectedClient(Socket socket) : base(socket)
        {

        }
    }
}