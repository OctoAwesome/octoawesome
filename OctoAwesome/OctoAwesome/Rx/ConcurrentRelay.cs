using OctoAwesome.Threading;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    public class ConcurrentRelay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<RelaySubscription> subscriptions;
        private readonly LockSemaphore lockSemaphore;

        public ConcurrentRelay()
        {
            lockSemaphore = new LockSemaphore(1, 1);
            subscriptions = new();
        }

        public void OnCompleted()
        {
            using var scope = lockSemaphore.Wait();

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            using var scope = lockSemaphore.Wait();

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            using var scope = lockSemaphore.Wait();

            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(this, observer);

            using (var scope = lockSemaphore.Wait())
                subscriptions.Add(sub);

            return sub;
        }

        public void Dispose()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }

            subscriptions.Clear();
            lockSemaphore.Dispose();
        }

        private void Unsubscribe(RelaySubscription subscription)
        {
            using var scope = lockSemaphore.Wait();

            subscriptions.Remove(subscription);
        }


        private class RelaySubscription : IDisposable
        {
            public IObserver<T> Observer { get; }

            private readonly ConcurrentRelay<T> relay;

            public RelaySubscription(ConcurrentRelay<T> relay, IObserver<T> observer)
            {
                this.relay = relay;

                Observer = observer;
            }

            public void Dispose()
            {
                relay.Unsubscribe(this);
            }
        }
    }
}
