using OctoAwesome.Network.ServerNotifications;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OctoAwesome.Network
{
    public class ConnectedClient : BaseClient, INotificationObserver
    {
        public IDisposable NetworkChannelSubscription { get; set; }
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
            OfficialCommand command;
            byte[] payload;
            switch (value)
            {
                case EntityNotification entityNotification:
                    command = OfficialCommand.EntityNotification;
                    payload = Serializer.Serialize(entityNotification, null);
                    break;
                default:
                    return;
            }

            BuildAndSendPackage(payload, command);
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