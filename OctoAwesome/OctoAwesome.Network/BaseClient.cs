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
using OctoAwesome.Logging;
using System.IO;

namespace OctoAwesome.Network
{
    /// <summary>
    /// OctoAwesome base client class.
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        private static uint NextId => ++nextId;
        private static uint nextId;

        /// <summary>
        /// Called when a the client has been disconnected from the server.
        /// </summary>
        public event EventHandler<EventArgs>? ClientDisconnected;

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
        private readonly ILogger logger;
        private readonly CancellationTokenSource cancellationTokenSource;

        private readonly ConcurrentRelay<Package> packages;
        private TcpClient? tcpClient;
        private NetworkStream stream;


        static BaseClient()
        {
            nextId = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseClient"/> class.
        /// </summary>
        /// <param name="socket">The low level base socket.</param>
        protected BaseClient(TcpClient socket)
        {
            packages = new ConcurrentRelay<Package>();

            packagePool = TypeContainer.Get<PackagePool>();
            logger = TypeContainer.Get<ILogger>().As(typeof(BaseClient));
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
            logger.Debug($"Starting connection to {TcpClient.Client.RemoteEndPoint}");
            stream = TcpClient.GetStream();

            var buffer = new byte[1024 * 1024];
            try
            {
                do
                {
                    var readBytes = await stream.ReadAsync(buffer, cancellationTokenSource.Token);

                    if (readBytes < 1)
                    {
                        //abort!
                        Stop();
                        return;
                    }

                    int offset = 0;
                    do
                    {
                        offset += DataReceived(buffer, readBytes, offset);
                        logger.Trace($"Recveided bytes new offest: {offset}");
                    } while (offset < readBytes);

                } while (true);
            }
            catch (IOException)
            {
                Stop();
                throw;
            }

        }

        /// <summary>
        /// Stops receiving data for this client.
        /// </summary>
        public void Stop()
        {
            logger.Debug($"Stopping connection {TcpClient.Client.RemoteEndPoint}");
            ClientDisconnected?.Invoke(this, EventArgs.Empty);
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
            logger.Debug($"Send package with id: {package.UId} and Flags: {package.PackageFlags} to client: {TcpClient.Client.RemoteEndPoint}");
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
        public async Task SendPackageAndRelease(Package package)
        {
            await SendPackageAsync(package).ConfigureAwait(false);//.GetAwaiter().GetResult();
            package.Release();
        }

        private async ValueTask SendInternal(byte[] data, int len)
        {
            try
            {
                await stream.WriteAsync(data.AsMemory(0, len));
                logger.Trace($"Did send the data with len {len}");
            }
            catch (IOException ex)
            {
                logger.Error(ex);
                Stop();
            }
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
                    var ex = new ArgumentOutOfRangeException(nameof(buffer), $"Buffer is to small for package head deserialization [buffersize: {buffer.Length} | length: {length} | offset: {bufferOffset}]");
                    ex.Data.Add("buffer.Length", buffer.Length);
                    ex.Data.Add(nameof(length), length);
                    ex.Data.Add(nameof(bufferOffset), bufferOffset);
                    logger.Error($"Buffer was to small", ex);
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
                        var ex = new InvalidCastException("Can not deserialize header with these bytes :(");
                        logger.Error($"Header deserialization failed", ex);
                        throw ex;
                    }
                }
            }

            offset += currentPackage.DeserializePayload(buffer, bufferOffset + offset, length - (bufferOffset + offset));

            if (currentPackage.IsComplete)
            {
                var package = currentPackage;

                logger.Trace($"Package {package} was complete, dispatching");
                packages.OnNext(package);
                currentPackage = null;
            }
            else
            {
                logger.Trace("Package was not complete, waiting for more bytes");
            }

            return offset;
        }

        public virtual void Dispose()
        {
            packages.Dispose();
            cancellationTokenSource.Dispose();
            stream?.Dispose();
        }
    }
}
