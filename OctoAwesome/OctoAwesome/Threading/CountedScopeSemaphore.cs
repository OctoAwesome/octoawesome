using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore : IDisposable
    {
        private readonly ManualResetEventSlim superLock;
        private readonly ManualResetEventSlim mainLock;

        private readonly object lockObject;
        private readonly object countLockObject;
        private int counter;
        public CountedScopeSemaphore()
        {
            mainLock = new ManualResetEventSlim(true);
            superLock = new ManualResetEventSlim(true);
            lockObject = new object();
            countLockObject = new object();
        }

        public SuperScope EnterExclusivScope()
        {
            lock (lockObject)
            {
                mainLock.Wait();
                superLock.Wait();
                superLock.Reset();
            }
            return new SuperScope(this);
        }

        public CountScope EnterCountScope()
        {
            lock (lockObject)
            {
                superLock.Wait();
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
            superLock.Dispose();
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

        private void LeaveSuperScope()
        {
            superLock.Set();
        }

        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            public static CountScope Empty => new CountScope(null);

            private readonly CountedScopeSemaphore internalSemaphore;

            public CountScope(CountedScopeSemaphore countingSemaphore)
            {
                internalSemaphore = countingSemaphore;
            }

            public void Dispose()
            {
                internalSemaphore?.LeaveMainScope();
            }

            public override bool Equals(object obj)
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

        public readonly struct SuperScope : IDisposable, IEquatable<SuperScope>
        {
            public static SuperScope Empty => new SuperScope(null);

            private readonly CountedScopeSemaphore internalSemaphore;

            public SuperScope(CountedScopeSemaphore semaphore)
            {
                internalSemaphore = semaphore;
            }

            public void Dispose()
            {
                internalSemaphore?.LeaveSuperScope();
            }

            public override bool Equals(object obj) => obj is SuperScope scope && Equals(scope);
            public bool Equals(SuperScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);
            public override int GetHashCode()
                => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            public static bool operator ==(SuperScope left, SuperScope right) => left.Equals(right);
            public static bool operator !=(SuperScope left, SuperScope right) => !(left == right);
        }
    }
}
