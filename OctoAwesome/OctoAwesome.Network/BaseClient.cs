using OctoAwesome.Network.Pooling;
using OctoAwesome.Rx;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using OctoAwesome.Extension;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace OctoAwesome.Network
{
    /// <summary>
    /// OctoAwesome base client class.
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        private static uint NextId => ++nextId;
        private static uint nextId;

        static BaseClient()
        {
            nextId = 0;
        }
        /// <summary>
        /// Gets the client id.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Observable for receiving packages.
        /// </summary>
        public IObservable<Package> Packages => packages;

        /// <summary>
        /// The underlying socket.
        /// </summary>
        protected TcpClient TcpClient
        {
            get => NullabilityHelper.NotNullAssert(tcpClient);
            set => tcpClient = NullabilityHelper.NotNullAssert(value);
        }


        private Package? currentPackage;
        private readonly PackagePool packagePool;

        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly ConcurrentRelay<Package> packages;
        private TcpClient? tcpClient;
        private NetworkStream stream;


        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="socket">The low level base socket.</param>
        protected BaseClient(TcpClient socket) 
        {
            packages = new ConcurrentRelay<Package>();

            packagePool = TypeContainer.Get<PackagePool>();

            cancellationTokenSource = new CancellationTokenSource();

            Id = NextId;
            TcpClient = socket;
            TcpClient.NoDelay = true;
        }

        /// <summary>
        /// Starts receiving data for this client asynchronously.
        /// </summary>
        /// <returns>The created receiving task.</returns>
        public async ValueTask Start()
        {
            stream = TcpClient.GetStream();

            var buffer = new byte[1024 * 1024];
            do
            {
                var readBytes = await stream.ReadAsync(buffer, cancellationTokenSource.Token);

                if (readBytes < 1)
                {
                    //abort!
                    return;
                }

                int offset = 0;
                do
                {
                    offset += DataReceived(buffer, readBytes, offset);
                } while (offset < readBytes);

            } while (true);

        }

        /// <summary>
        /// Stops receiving data for this client.
        /// </summary>
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Send raw byte data asynchronously.
        /// </summary>
        /// <param name="data">The byte buffer to send data from.</param>
        /// <param name="len">The slice length of the data to send.</param>
        /// <returns>The created sending task.</returns>
        public ValueTask SendAsync(byte[] data, int len)
        {

            return SendInternal(data, len);
        }

        /// <summary>
        /// Send a package asynchronously.
        /// </summary>
        /// <param name="package">The package to send asynchronously.</param>
        public async ValueTask SendPackageAsync(Package package)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes, 0);
            await SendAsync(bytes, bytes.Length);
        }

        /// <summary>
        /// Send a package asynchronously and releases it into the memory pool.
        /// </summary>
        /// <param name="package">The package to send asynchronously.</param>
        /// <seealso cref="SendPackageAndRelease"/>
        public async ValueTask SendPackageAndReleaseAsync(Package package)
        {
            await SendPackageAsync(package);
            package.Release();
        }

        /// <summary>
        /// Send a package and releases it into the memory pool.
        /// </summary>
        /// <param name="package">The package to send asynchronously.</param>
        /// <seealso cref="SendPackageAndReleaseAsync"/>
        public void SendPackageAndRelease(Package package)
        {
            var task = Task.Run(async () => await SendPackageAsync(package));
            task.Wait();
            package.Release();
        }

        private async ValueTask SendInternal(byte[] data, int len)
        {
                await stream.WriteAsync(data.AsMemory(0, len));
                Console.WriteLine("Send package");

        }


        private int DataReceived(Span<byte> buffer, int length, int bufferOffset)
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

        /// <inheritdoc />
        public virtual void Dispose()
        {
            packages.Dispose();
            cancellationTokenSource.Dispose();
        }
    }
}
