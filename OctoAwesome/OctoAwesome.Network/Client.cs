using System.IO;
using System.Net.Sockets;

namespace OctoAwesome.Network
{
    public class Client
    {
        //public NetworkStream TcpStream => tcpClient.GetStream();

        public Socket Socket { get; private set; }

        private SocketAsyncEventArgs socketArgs;

        public Client(Socket socket)
        {
            Socket = socket;
            socketArgs = new SocketAsyncEventArgs();
            socketArgs.Completed += SocketArgsCompleted;
        }
        
        public void ReceiveAsync()
        {
            Socket.ReceiveAsync(socketArgs);
        }

        public void SendAsync(byte[] data)
        {
            socketArgs.SetBuffer(data, 0, data.Length);
            Socket.SendAsync(socketArgs);
        }

        private void SocketArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
        }

    }
}