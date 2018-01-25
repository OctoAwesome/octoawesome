using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class Client
    {
        public NetworkStream Stream => tcpClient.GetStream();
        
        private TcpClient tcpClient;

        public Client(TcpClient client)
        {
            tcpClient = client;
        }

        public async Task<byte[]> ReceiveAsync()
        {
            var data = new byte[tcpClient.Available];
            await Stream.ReadAsync(data, 0, data.Length);
            return data;
        }

        public async Task SendAsync(byte[] data) => await Stream.WriteAsync(data, 0, data.Length);
        
    }
}