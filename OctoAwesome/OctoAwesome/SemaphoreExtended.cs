using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public sealed class SemaphoreExtended : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim;

        public SemaphoreExtended(int initialCount, int maxCount)
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

        public struct SemaphoreLock : IDisposable
        {
            public static SemaphoreLock Empty => new SemaphoreLock(null);

            private readonly SemaphoreExtended internalSemaphore;

            public SemaphoreLock(SemaphoreExtended semaphoreExtended)
            {
                internalSemaphore = semaphoreExtended;
            }

            public void Dispose()
            {
                internalSemaphore?.Release();
            }
        }
    }
}
