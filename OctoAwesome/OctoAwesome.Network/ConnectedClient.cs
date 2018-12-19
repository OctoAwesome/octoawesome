using OctoAwesome.Network.ServerNotifications;
using OctoAwesome.Notifications;
using System;
using System.Buffers;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OctoAwesome.Network
{
    public class ConnectedClient : BaseClient, IUpdateSubscriber
    {
        public IDisposable ProviderSubscription { get; set; }
        public IDisposable ServerSubscription { get; set; }

        public ConnectedClient(Socket socket) : base(socket)
        {

        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Socket.Close();
            throw error;            
        }

        public void OnNext(Notification value)
        {
            switch (value)
            {
                case ServerDataNotification serverDataNotification:
                    if (serverDataNotification.Match(0))
                        BuildAndSendPackage(serverDataNotification.Data, serverDataNotification.OfficialCommand);
                    break;
                default:
                    break;
            }
        }

        private void BuildAndSendPackage(byte[] data, OfficialCommand officialCommand)
        {
            SendPackage(new Package()
            {
                Payload = data,
                Command = (ushort)officialCommand
            });
        }
    }
}