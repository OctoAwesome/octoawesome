using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OctoAwesome.Database.Threading
{
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

        public bool CheckLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if (exclusiveLocks)
                    return false;

                if (operation.HasFlag(Operation.Read))
                    return writeLocks < 1;

                if (operation.HasFlag(Operation.Write))
                    return readLocks < 1;
            }
            finally
            {
                semaphoreSlim.Release();
            }

            return true;
        }

        public void Wait(Operation operation)
        {
            //if (operation.HasFlag(Operation.Exclusive))
            //    exclusiveEvent.WaitOne();

            if (operation.HasFlag(Operation.Read))
                writeEvent.WaitOne();

            if (operation.HasFlag(Operation.Write))
                readEvent.WaitOne();

        }

        internal DatabaseOperation StartOperation(Operation operation)
        {
            Wait(operation);
            semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Read))
                    ++readOperations;

                if (operation.HasFlag(Operation.Write))
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
                if (operation.HasFlag(Operation.Read))
                    --readOperations;

                if (operation.HasFlag(Operation.Write))
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

        public void SetLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Exclusive))
                    exclusiveLocks = true;

                if (operation.HasFlag(Operation.Read))
                    ++readLocks;

                if (operation.HasFlag(Operation.Write))
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

        public void ReleaseLock(Operation operation)
        {
            semaphoreSlim.Wait();
            try
            {
                if (operation.HasFlag(Operation.Exclusive))
                    exclusiveLocks = false;

                if (operation.HasFlag(Operation.Read))
                    --readLocks;

                if (operation.HasFlag(Operation.Write))
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

        public void Dispose()
        {
            readEvent.Dispose();
            writeEvent.Dispose();
            exclusiveEvent.Dispose();
            semaphoreSlim.Dispose();
        }
    }
}
