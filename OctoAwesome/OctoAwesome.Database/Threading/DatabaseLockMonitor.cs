using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OctoAwesome.Database.Threading
{
    internal sealed class DatabaseLockMonitor
    {
        private int readLock;
        private int writeLock;
        private int exclusiveLock;

        public DatabaseLockMonitor()
        {
            readLock = 0;
            writeLock = 0;
            exclusiveLock = 0;
        }

        public bool CheckLock(Operation operation)
        {
            if (exclusiveLock > 0)
                return false;

            if (operation.HasFlag(Operation.Read))
                return writeLock < 1;

            if (operation.HasFlag(Operation.Write))
                return readLock < 1;

            return true;
        }

        public void SetLock(Operation operation)
        {
            if (operation.HasFlag(Operation.Exclusive))
                Interlocked.Increment(ref exclusiveLock);

            if (operation.HasFlag(Operation.Read))
                Interlocked.Increment(ref readLock);

            if (operation.HasFlag(Operation.Write))
                Interlocked.Increment(ref writeLock);
        }

        public void ReleaseLock(Operation operation)
        {
            if (operation.HasFlag(Operation.Exclusive))
                Interlocked.Decrement(ref exclusiveLock);

            if (operation.HasFlag(Operation.Read))
                Interlocked.Decrement(ref readLock);

            if (operation.HasFlag(Operation.Write))
                Interlocked.Decrement(ref writeLock);
        }
    }
}
