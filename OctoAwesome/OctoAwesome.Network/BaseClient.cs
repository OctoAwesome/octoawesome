using OctoAwesome.Network.Pooling;
using OctoAwesome.Rx;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{

    public abstract class BaseClient : IDisposable
    {
        private static uint NextId => ++nextId;
        private static uint nextId;

        static BaseClient()
        {
            nextId = 0;
        }

        public uint Id { get; }
        public IObservable<Package> Packages => packages;
        protected Socket Socket;
        protected readonly SocketAsyncEventArgs ReceiveArgs;

        private byte readSendQueueIndex;
        private byte nextSendQueueWriteIndex;
        private bool sending;
        private Package currentPackage;
        private readonly PackagePool packagePool;
        private readonly SocketAsyncEventArgs sendArgs;

        private readonly (byte[] data, int len)[] sendQueue;
        private readonly object sendLock;
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly ConcurrentRelay<Package> packages;
        protected BaseClient()
        {
            packages = new ConcurrentRelay<Package>();

            sendQueue = new (byte[] data, int len)[256];
            sendLock = new object();
            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.Completed += OnReceived;
            ReceiveArgs.SetBuffer(ArrayPool<byte>.Shared.Rent(1024 * 1024), 0, 1024 * 1024);
            packagePool = TypeContainer.Get<PackagePool>();

            sendArgs = new SocketAsyncEventArgs();
            sendArgs.Completed += OnSent;

            cancellationTokenSource = new CancellationTokenSource();

            Id = NextId;
        }

        protected BaseClient(Socket socket) : this()
        {
            Socket = socket;
            Socket.NoDelay = true;
        }

        public Task Start()
        {
            return Task.Run(() =>
            {
                if (Socket.ReceiveAsync(ReceiveArgs))
                    return;

                Receive(ReceiveArgs);
            }, cancellationTokenSource.Token);
        }
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public Task SendAsync(byte[] data, int len)
        {
            lock (sendLock)
            {
                if (sending)
                {
                    sendQueue[nextSendQueueWriteIndex++] = (data, len);
                    return Task.CompletedTask;
                }

                sending = true;
            }

            return Task.Run(() => SendInternal(data, len));
        }

        public async Task SendPackageAsync(Package package)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes, 0);
            await SendAsync(bytes, bytes.Length);
        }

        public async Task SendPackageAndReleaseAsync(Package package)
        {
            await SendPackageAsync(package);
            package.Release();
        }

        public void SendPackageAndRelease(Package package)
        {
            var task = Task.Run(async () => await SendPackageAsync(package));
            task.Wait();
            package.Release();
        }

        private void SendInternal(byte[] data, int len)
        {
            while (true)
            {
                sendArgs.SetBuffer(data, 0, len);

                Console.WriteLine("Send now a package");
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

        private void OnReceived(object? sender, SocketAsyncEventArgs e)
        {
            Receive(e);
        }

        protected void Receive(SocketAsyncEventArgs e)
        {
            do
            {
                if (e.BytesTransferred < 1)
                    return;

                int offset = 0;

                do
                {
                    offset += DataReceived(e.Buffer, e.BytesTransferred, offset);

                } while (offset < e.BytesTransferred);

            } while (!Socket.ReceiveAsync(e));
        }

        private int DataReceived(byte[] buffer, int length, int bufferOffset)
        {
            int offset = 0;

            if (currentPackage == null)
            {
                currentPackage = packagePool.GetBlank();
                currentPackage.BaseClient = this;

                if (length - bufferOffset < Package.HEAD_LENGTH)
                {
                    var ex = new Exception($"Buffer is to small for package head deserialization [length: {length} | offset: {bufferOffset}]");
                    ex.Data.Add(nameof(length), length);
                    ex.Data.Add(nameof(bufferOffset), bufferOffset);
                    throw ex;
                }
                else
                {
                    if (currentPackage.TryDeserializeHeader(buffer, bufferOffset))
                    {
                        offset += Package.HEAD_LENGTH;
                    }
                    else
                    {
                        throw new InvalidCastException("Can not deserialize header with these bytes :(");
                    }
                }
            }

            offset += currentPackage.DeserializePayload(buffer, bufferOffset + offset, length - (bufferOffset + offset));

            if (currentPackage.IsComplete)
            {
                var package = currentPackage;

                Debug.WriteLine("Package:  " + package.UId);
                packages.OnNext(package);
                currentPackage = null;
            }

            return offset;
        }
        public virtual void Dispose()
        {
            packages?.Dispose();
            ReceiveArgs?.Dispose();
            sendArgs?.Dispose();
            cancellationTokenSource?.Dispose();
        }
    }
}
