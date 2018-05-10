using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public abstract class BaseClient
    {
        public event EventHandler<(byte[] Data, int Count)> OnMessageRecived;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;

        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        protected BaseClient(Socket socket)
        {
            sendQueue = new(byte[] data, int len)[256];
            sendLock = new object();

            Socket = socket;
            Socket.NoDelay = true;

            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(2048), 0, 2048);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

        }

        public void Start()
        {
            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                ProcessInternal(ReceiveArgs.Buffer, ReceiveArgs.BytesTransferred);
            }
        }

        public void SendAsync(byte[] data, int len)
        {
            lock (sendLock)
            {
                if (sending)
                {
                    sendQueue[nextSendQueueWriteIndex++] = (data, len);
                    return;
                }

                sending = true;
            }

            SendInternal(data, len);

        }
        public void SendAsync(Package package)
        {
            var buffer = new byte[2048];
            
            var len = package.Read(buffer, package.Payload.Length > 2000);

            SendAsync(buffer, len);
        }

        public Package SendAndReceive(Package package)
        {
            var manualResetEvent = new ManualResetEvent(false);
            Package returnPackage = null;
            var onDataReceive = new EventHandler<(byte[] Data, int Count)>((sender, eventArgs) =>
            {
                returnPackage = new Package(eventArgs.Data.Take(eventArgs.Count).ToArray());

                manualResetEvent.Set();
            });
            OnMessageRecived += onDataReceive;

            SendAsync(package);
            manualResetEvent.WaitOne();

            OnMessageRecived -= onDataReceive;

            return returnPackage;
        }

        protected abstract void ProcessInternal(byte[] receiveArgsBuffer, int receiveArgsCount);

        protected void OnMessageReceivedInvoke(byte[] receiveArgsBuffer, int receiveArgsCount)
            => OnMessageRecived?.Invoke(this, (receiveArgsBuffer, receiveArgsCount));

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(sendArgs))
                    return;

                ArrayPool<byte>.Shared.Return(data);

                lock (sendLock)
                {
                    if (readSendQueueIndex < nextSendQueueWriteIndex)
                    {
                        (data, len) = sendQueue[readSendQueueIndex++];
                    }
                    else
                    {
                        nextSendQueueWriteIndex = 0;
                        readSendQueueIndex = 0;
                        sending = false;
                        return;
                    }
                }
            }
        }

        private void OnSent(object sender, SocketAsyncEventArgs e)
        {
            byte[] data;
            int len;

            ArrayPool<byte>.Shared.Return(e.Buffer);

            lock (sendLock)
            {
                if (readSendQueueIndex < nextSendQueueWriteIndex)
                {
                    (data, len) = sendQueue[readSendQueueIndex++];
                }
                else
                {
                    nextSendQueueWriteIndex = 0;
                    readSendQueueIndex = 0;
                    sending = false;
                    return;
                }
            }

            SendInternal(data, len);
        }

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            ProcessInternal(e.Buffer, e.BytesTransferred);

            while (Socket.Connected)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                ProcessInternal(ReceiveArgs.Buffer, ReceiveArgs.BytesTransferred);
            }
        }
    }
}
