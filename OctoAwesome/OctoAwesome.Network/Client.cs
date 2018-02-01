using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Buffers;
using System.Net;

namespace OctoAwesome.Network
{
    public class Client
    {
        public event EventHandler<(byte[] data, int length)> OnMessageRecived;

        private readonly Socket socket;
        private readonly SocketAsyncEventArgs receiveArgs;
        private readonly SocketAsyncEventArgs sendArgs;

        private readonly Queue<byte[]> queue;
        private readonly object lockObject;

        public bool Connected => socket.Connected;

        public Client(Socket socket)
        {
            lockObject = new object();
            queue = new Queue<byte[]>();

            this.socket = socket;

            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.Completed += ReceiveArgsCompleted;
            receiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(2048), 0, 2048);

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += SendArgsCompleted; ;
            sendArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(2048), 0, 2048);

        }
        public Client() : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            NoDelay = true
        })
        { }

        public void Send(byte[] data)
        {
            lock (lockObject)
            {
                queue.Enqueue(data);
            }
            
            SendInternal(data);
        }

        public void Connect(string host, int port)
            => socket.BeginConnect(new IPEndPoint(IPAddress.Parse(host), port), OnConnect, null);



        public void Listening()
        {
            while (true)
            {
                if (socket.ReceiveAsync(receiveArgs))
                    return;

                ProcessInternal(receiveArgs.Buffer, receiveArgs.BytesTransferred);
            }
        }

        private void ProcessInternal(byte[] buffer, int bytesTransferred)
        {
            OnMessageRecived?.Invoke(this, (buffer, bytesTransferred));
        }

        private void OnConnect(IAsyncResult ar)
        {
            socket.EndConnect(ar);

            while (true)
            {
                if (socket.ReceiveAsync(receiveArgs))
                    return;

                ProcessInternal(receiveArgs.Buffer, receiveArgs.BytesTransferred);
            }
        }

        private void SendInternal(byte[] data)
        {
            while (true)
            {
                Buffer.BlockCopy(data, 0, sendArgs.Buffer, 0, data.Length);
                sendArgs.SetBuffer(0, data.Length);

                //ArrayPool<byte>.Shared.Return(data);

                if (socket.SendAsync(sendArgs))
                    return;

                lock (lockObject)
                {
                    if (queue.Count > 0)
                        data = queue.Dequeue();
                    else
                        return;
                }
            }
        }

        private void SendArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            byte[] data;

            lock (lockObject)
            {
                if (queue.Count > 0)
                    data = queue.Dequeue();
                else
                    return;

            }
        }

        private void ReceiveArgsCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessInternal(e.Buffer, e.BytesTransferred);

            while (true)
            {
                if (socket.ReceiveAsync(receiveArgs))
                    return;

                ProcessInternal(receiveArgs.Buffer, receiveArgs.BytesTransferred);
            }
        }
    }
}