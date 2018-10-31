using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public abstract class BaseClient : ObservableBase<OctoNetworkEventArgs>
    {
        //public delegate int ReceiveDelegate(object sender, (byte[] Data, int Offset, int Count) eventArgs);
        //public event ReceiveDelegate OnMessageRecived;

        protected readonly Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        protected readonly OctoNetworkStream internalSendStream;
        protected readonly OctoNetworkStream internalRecivedStream;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;
        private readonly List<IObserver<OctoNetworkEventArgs>> observers;
        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;

        protected BaseClient(Socket socket)
        {
            sendQueue = new (byte[] data, int len)[256];
            sendLock = new object();

            Socket = socket;
            Socket.NoDelay = true;

            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(2048), 0, 2048);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

            internalSendStream = new OctoNetworkStream();
            internalRecivedStream = new OctoNetworkStream();

            observers = new List<IObserver<OctoNetworkEventArgs>>();

        }

        public void Start()
        {
            while (true)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;
                Receive(ReceiveArgs);
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


        protected override IDisposable SubscribeCore(IObserver<OctoNetworkEventArgs> observer)
        {
            observers.Add(observer);
            return observer as IDisposable;
        }

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                if (Socket.SendAsync(sendArgs))
                    return;

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

        protected void Receive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred < 1)
                return;

            int offset = 0;
            int count = 0;
            do
            {
                count = internalRecivedStream.Write(e.Buffer, offset, e.BytesTransferred - offset);

                if (count > 0)
                    Notify(new OctoNetworkEventArgs { Client = this, NetworkStream = internalRecivedStream, DataCount = count });


                offset += count;
            } while (offset < e.BytesTransferred);
        }

        private void OnReceived(object sender, SocketAsyncEventArgs e)
        {
            Receive(e);

            while (Socket.Connected)
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }
        }

        protected virtual void Notify(OctoNetworkEventArgs args)
        {
            Parallel.ForEach(observers, (o) => o.OnNext(args));
            //observers.ForEach(o => o.OnNext(args));
        }


    }
}
