using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    public class CountedScopeSemaphore
    {
        private readonly SemaphoreSlim semaphoreSlim;

        private volatile int counter;

        public CountedScopeSemaphore(int initialCount)
        {
            semaphoreSlim = new SemaphoreSlim(1, 1);
            counter = initialCount;
        }

        public void Wait()
        {
            if (counter > 0)
                semaphoreSlim.Wait();
        }

        public CountScope EnterScope()
        {
            counter++;
            return new CountScope(this);
        }

        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }

        private void LeaveScope()
        {
            counter--;

            if (counter == 0 && semaphoreSlim.CurrentCount < 1)
                semaphoreSlim.Release();
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
                internalSemaphore?.LeaveScope();
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
    }
}
