using System;
using System.Collections.Generic;
using System.Threading;

namespace OctoAwesome.Threading
{

    public class CountedScopeSemaphore : IDisposable
    {
        private readonly ManualResetEventSlim exclusiveLock;
        private readonly ManualResetEventSlim mainLock;

        private readonly object lockObject;
        private readonly object countLockObject;
        private int counter;

        public CountedScopeSemaphore()
        {
            mainLock = new ManualResetEventSlim(true);
            exclusiveLock = new ManualResetEventSlim(true);
            lockObject = new object();
            countLockObject = new object();
        }
        public ExclusiveScope EnterExclusiveScope()
        {
            lock (lockObject)
            {
                mainLock.Wait();
                exclusiveLock.Wait();
                exclusiveLock.Reset();
            }
            return new ExclusiveScope(this);
        }
        public CountScope EnterCountScope()
        {
            lock (lockObject)
            {
                exclusiveLock.Wait();
                lock (countLockObject)
                {
                    counter++;
                    if (counter > 0)
                        mainLock.Reset();
                }
            }
            
            return new CountScope(this);
        }
        public void Dispose()
        {
            exclusiveLock.Dispose();
            mainLock.Dispose();
        }

        private void LeaveMainScope()
        {
            lock (countLockObject)
            {
                counter--;
                if (counter == 0)
                    mainLock.Set();
            }
        }

        private void LeaveExclusiveScope()
        {
            exclusiveLock.Set();
        }
        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            private readonly CountedScopeSemaphore internalSemaphore;

            public CountScope(CountedScopeSemaphore countingSemaphore)
            {
                internalSemaphore = countingSemaphore;
            }
            public void Dispose()
            {
                internalSemaphore.LeaveMainScope();
            }
            public override bool Equals(object? obj)
                => obj is CountScope scope
                  && Equals(scope);
            public bool Equals(CountScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);
            public override int GetHashCode()
                => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            public static bool operator ==(CountScope left, CountScope right)
                => left.Equals(right);
            
            public static bool operator !=(CountScope left, CountScope right)
                => !(left == right);
        }
        public readonly struct ExclusiveScope : IDisposable, IEquatable<ExclusiveScope>
        {
            private readonly CountedScopeSemaphore internalSemaphore;

            public ExclusiveScope(CountedScopeSemaphore semaphore)
            {
                internalSemaphore = semaphore;
            }
            public void Dispose()
            {
                internalSemaphore?.LeaveExclusiveScope();
            }
            public override bool Equals(object? obj) => obj is ExclusiveScope scope && Equals(scope);
            public bool Equals(ExclusiveScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);
            public override int GetHashCode()
                => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);
            
            public static bool operator ==(ExclusiveScope left, ExclusiveScope right) => left.Equals(right);
            
            public static bool operator !=(ExclusiveScope left, ExclusiveScope right) => !(left == right);
        }
    }
}
