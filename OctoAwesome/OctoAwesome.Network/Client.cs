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
        public bool IsClient { get; set; }

        private static int clientReceived;

        public Client() :
            base(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        public void SendPing()
        {
            var buffer = ArrayPool<byte>.Shared.Rent(4);
            Encoding.UTF8.GetBytes("PING", 0, 4, buffer, 0);
            SendAsync(buffer, 4);
        }

        public void Connect(string host, ushort port)
        {
            var address = Dns.GetHostAddresses(host).FirstOrDefault(
                a => a.AddressFamily == Socket.AddressFamily);

            Socket.BeginConnect(new IPEndPoint(address, port), OnConnected, null);
        }

        protected override int ProcessInternal(byte[] receiveArgsBuffer,int receiveOffset, int receiveArgsCount)
        {
            int read = base.ProcessInternal(receiveArgsBuffer, receiveOffset, receiveArgsCount);

            var tmpString = Encoding.UTF8.GetString(receiveArgsBuffer, 0, receiveArgsCount);

            var increment = Interlocked.Increment(ref clientReceived);
            if (increment > 0 && increment % 10000 == 0)
                Console.WriteLine($"CLIENTS Received 10000 messages");
            return read;
        }

        private void OnConnected(IAsyncResult ar)
        {
            Socket.EndConnect(ar);

            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;
                int offset = 0;
                do
                {
                    offset += ProcessInternal(ReceiveArgs.Buffer, offset, ReceiveArgs.BytesTransferred);
                } while (offset < ReceiveArgs.BytesTransferred);
            }
        }
    }
}