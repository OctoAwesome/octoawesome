using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    public class Relay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<RelaySubscription> subscriptions;
        public Relay()
        {
            subscriptions = new();
        }
        public void OnCompleted()
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i].Observer.OnCompleted();
            }
        }
        public void OnError(Exception error)
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i].Observer.OnError(error);
            }
        }
        public void OnNext(T value)
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i].Observer.OnNext(value);
            }
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(this, observer);
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
        }

        private void Unsubscribe(RelaySubscription subscription)
        {
            subscriptions.Remove(subscription);
        }
        private class RelaySubscription : IDisposable
        {
            public IObserver<T> Observer { get; }

            private readonly Relay<T> relay;

            public RelaySubscription(Relay<T> relay, IObserver<T> observer)
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
