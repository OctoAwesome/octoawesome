using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    public class BehaviorRelay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<BehaviorRelaySubscription> subscriptions;

        private T lastValue;

        public BehaviorRelay(T initialValue)
        {
            subscriptions = new();
            lastValue = initialValue;
        }

        public void OnCompleted()
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            lastValue = value;
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new BehaviorRelaySubscription(this, observer);
            subscriptions.Add(sub);
            sub.Observer.OnNext(lastValue);
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

        private void Unsubscribe(BehaviorRelaySubscription subscription)
        {
            subscriptions.Remove(subscription);
        }


        private class BehaviorRelaySubscription : IDisposable
        {
            public IObserver<T> Observer { get; }

            private readonly BehaviorRelay<T> relay;

            public BehaviorRelaySubscription(BehaviorRelay<T> relay, IObserver<T> observer)
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
