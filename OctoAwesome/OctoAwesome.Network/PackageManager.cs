using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class PackageManager : IObserver<OctoNetworkEventArgs>
    {
        public event EventHandler<OctoPackageAvailableEventArgs> PackageAvailable;


        private readonly List<Subscription<OctoNetworkEventArgs>> subsciptions;
        private readonly Dictionary<BaseClient, Package> packages;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly ConcurrentQueue<OctoNetworkEventArgs> receivingQueue;
        private readonly MemoryStream backupStream;


        public PackageManager()
        {
            packages = new Dictionary<BaseClient, Package>();
            subsciptions = new List<Subscription<OctoNetworkEventArgs>>();
            receivingQueue = new ConcurrentQueue<OctoNetworkEventArgs>();
            cancellationTokenSource = new CancellationTokenSource();
            backupStream = new MemoryStream();
        }

        public void AddConnectedClient(BaseClient client)
        {
           subsciptions.Add((Subscription<OctoNetworkEventArgs>)client.Subscribe(this));
        }

        public void SendPackage(Package package, BaseClient client)
        {
            byte[] bytes = new byte[package.Payload.Length + Package.HEAD_LENGTH];
            package.SerializePackage(bytes);
            client.SendAsync(bytes, bytes.Length);
        }

        public void OnNext(OctoNetworkEventArgs value)
            => receivingQueue.Enqueue(value);

        public void OnError(Exception error) => throw new NotImplementedException();

        public void OnCompleted() => throw new NotImplementedException();

        public Task Start()
        {
            var task = new Task(InternalProcess, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
            task.Start(TaskScheduler.Default);
            return task;
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        private void InternalProcess()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                if (receivingQueue.IsEmpty)
                    continue;

                if (receivingQueue.TryDequeue(out OctoNetworkEventArgs eventArgs))
                    ClientDataAvailable(eventArgs);
            }
        }

        private void ClientDataAvailable(OctoNetworkEventArgs e)
        {
            var baseClient = e.Client;

            var data = e.NetworkStream.DataAvailable(Package.HEAD_LENGTH);

            byte[] bytes;
            bytes = new byte[e.DataCount];

            if (!packages.TryGetValue(baseClient, out Package package))
            {
                int offset = 0;

                if (backupStream.Length > 0)
                {
                    e.DataCount += (int)backupStream.Length;
                    backupStream.Read(bytes, 0, (int)backupStream.Length);
                    offset = (int)backupStream.Length;
                    backupStream.Position = 0;
                    backupStream.SetLength(0);
                }

                data += offset;

                if (data < Package.HEAD_LENGTH)
                {
                    e.NetworkStream.Read(bytes, offset, data);
                    backupStream.Write(bytes, 0, data);
                    backupStream.Position = 0;
                    return;
                }

                package = new Package(false);


                offset += e.NetworkStream.Read(bytes, offset, Package.HEAD_LENGTH - offset);

                if (package.TryDeserializeHeader(bytes))
                {
                    packages.Add(baseClient, package);
                    e.DataCount -= Package.HEAD_LENGTH;
                }
                else
                {
                    throw new InvalidDataException("Can not deserialize header with these bytes :(");
                }

            }

            int count = package.PayloadRest;

            if ((e.DataCount - count) < 1)
                count = e.DataCount;

            if (count > 0)
                count = e.NetworkStream.Read(bytes, 0, count);

            count = package.DeserializePayload(bytes, 0, count);

            if (package.IsComplete)
            {
                packages.Remove(baseClient);
                PackageAvailable?.Invoke(this, new OctoPackageAvailableEventArgs { BaseClient = baseClient, Package = package });

                if (e.DataCount - count > 0)
                    ClientDataAvailable(new OctoNetworkEventArgs() { Client = baseClient, DataCount = e.DataCount - count, NetworkStream = e.NetworkStream });
            }
        }

    }
}
