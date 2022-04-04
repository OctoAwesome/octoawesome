using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{

    public sealed class LockSemaphore : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim;


        public LockSemaphore(int initialCount, int maxCount)
        {
            semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);
        }

        public SemaphoreLock Wait()
        {
            semaphoreSlim.Wait();
            return new SemaphoreLock(this);
        }

        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await semaphoreSlim.WaitAsync(token);
            return new SemaphoreLock(this);
        }
        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }

        private void Release()
        {
            semaphoreSlim.Release();
        }
        public readonly struct SemaphoreLock : IDisposable, IEquatable<SemaphoreLock>
        {
            private readonly LockSemaphore internalSemaphore;

            public SemaphoreLock(LockSemaphore semaphoreExtended)
            {
                internalSemaphore = semaphoreExtended;
            }
            public void Dispose()
            {
                internalSemaphore.Release();
            }
            public override bool Equals(object? obj)
                => obj is SemaphoreLock @lock
                   && Equals(@lock);
            public bool Equals(SemaphoreLock other)
                => EqualityComparer<LockSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);
            public override int GetHashCode()
                => 37286538 + (internalSemaphore == null ? 0 : EqualityComparer<LockSemaphore>.Default.GetHashCode(internalSemaphore));

            public static bool operator ==(SemaphoreLock left, SemaphoreLock right)
                => left.Equals(right);

            public static bool operator !=(SemaphoreLock left, SemaphoreLock right)
                => !(left == right);
        }
    }
}
