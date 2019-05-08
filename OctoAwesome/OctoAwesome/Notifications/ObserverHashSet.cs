using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class ObserverHashSet : HashSet<INotificationObserver>
    {
        private readonly SemaphoreSlim semaphore;

        public ObserverHashSet() : base()
        {
            semaphore = new SemaphoreSlim(1, 1);
        }

        public ObserverHashSet(IEqualityComparer<INotificationObserver> comparer) :  base(comparer)
        {
            semaphore = new SemaphoreSlim(1, 1);
        }

        public ObserverHashSet(IEnumerable<INotificationObserver> collection) : base(collection)
        {
            semaphore = new SemaphoreSlim(1, 1);
        }

        public ObserverHashSet(IEnumerable<INotificationObserver> collection, IEqualityComparer<INotificationObserver> comparer)
            : base(collection, comparer)
        {
            semaphore = new SemaphoreSlim(1, 1);
        }

        public void Wait()
        {
            semaphore.Wait();
        }

        public void Release()
        {
            semaphore.Release();
        }
    }
}
