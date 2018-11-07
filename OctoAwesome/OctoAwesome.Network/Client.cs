using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Buffers;
using System.Net;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.NetworkInformation;

namespace OctoAwesome.Network
{
    public class Client : BaseClient
    {        
        public Client() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        public void Connect(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == Socket.AddressFamily);

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

        internal void SendPackage(Package package)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            SendAsync(bytes, bytes.Length);
        }

    }
}