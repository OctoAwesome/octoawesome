using System;
using System.Threading;

namespace OctoAwesome.Database.Threading
{
    /// <summary>
    /// Helper class for managing locks on databases.
    /// </summary>
    public sealed class DatabaseLockMonitor : IDisposable
    {
        private int readLocks;
        private int writeLocks;
        private bool exclusiveLocks;

        private int readOperations;
        private int writeOperations;

        private readonly ManualResetEvent readEvent;
        private readonly ManualResetEvent writeEvent;
        private readonly ManualResetEvent exclusiveEvent;
        private readonly SemaphoreSlim semaphoreSlim;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseLockMonitor"/>.
        /// </summary>
        public DatabaseLockMonitor()
        {
            readEvent = new ManualResetEvent(true);
            writeEvent = new ManualResetEvent(true);
            exclusiveEvent = new ManualResetEvent(true);
            semaphoreSlim = new SemaphoreSlim(1, 1);

            readLocks = 0;
            writeLocks = 0;
            readOperations = 0;
            writeOperations = 0;
            exclusiveLocks = false;
        }

        /// <summary>
        /// Checks whether <see cref="Operation"/> flag types can be executed in the current lock state.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> flag types to check for.</param>
        /// <returns>
        /// A value indicating whether <see cref="Operation"/> flag types can be executed in the current lock state.
        /// </returns>
        public bool CheckLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if (exclusiveLocks)
                    return false;

                if ((operation & Operation.Read) == Operation.Read)
                    return writeLocks < 1;

                if ((operation & Operation.Write) == Operation.Write)
                    return readLocks < 1;
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return true;
        }

        /// <summary>
        /// Wait till <see cref="Operation"/> flag types lock can be acquired.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> flag types lock to wait for to be acquirable.</param>
        public void Wait(Operation operation)
        {
            //if (operation.HasFlag(Operation.Exclusive))
            //    exclusiveEvent.WaitOne();

            if ((operation & Operation.Read) == Operation.Read)
                writeEvent.WaitOne();

            if ((operation & Operation.Write) == Operation.Write)
                readEvent.WaitOne();

        }

        internal DatabaseOperation StartOperation(Operation operation)
        {
            Wait(operation);
            semaphoreSlim.Wait();
            try
            {
                if ((operation & Operation.Read) == Operation.Read)
                    ++readOperations;

                if ((operation & Operation.Write) == Operation.Write)
                    ++writeOperations;

                return new DatabaseOperation(this, operation);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        internal void StopOperation(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if ((operation & Operation.Read) == Operation.Read)
                    --readOperations;

                if ((operation & Operation.Write) == Operation.Write)
                    --writeOperations;

                if (readLocks == 0 && readOperations == 0)
                    writeEvent.Set();

                if (writeLocks == 0 && writeOperations == 0)
                    readEvent.Set();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Acquire a lock for a specified <see cref="Operation"/> flag types.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> flag types to acquire the lock for.</param>
        public void AcquireLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if ((operation & Operation.Exclusive) == Operation.Exclusive)
                    exclusiveLocks = true;

                if ((operation & Operation.Read) == Operation.Read)
                    ++readLocks;

                if ((operation & Operation.Write) == Operation.Write)
                    ++writeLocks;

                if (exclusiveLocks)
                {
                    exclusiveEvent.Reset();
                    return;
                }

                if (readLocks > 0)
                    readEvent.Reset();

                if (writeLocks > 0)
                    writeEvent.Reset();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Release a lock for a specified <see cref="Operation"/> flag types.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> flag types to release the lock for.</param>
        public void ReleaseLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if ((operation & Operation.Exclusive) == Operation.Exclusive)
                    exclusiveLocks = false;

                if ((operation & Operation.Read) == Operation.Read)
                    --readLocks;

                if ((operation & Operation.Write) == Operation.Write)
                    --writeLocks;

                if (readLocks == 0 && readOperations == 0)
                    readEvent.Set();

                if (writeLocks == 0 && writeOperations == 0)
                    writeEvent.Set();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            readEvent.Dispose();
            writeEvent.Dispose();
            exclusiveEvent.Dispose();
            semaphoreSlim.Dispose();
        }
    }
}
