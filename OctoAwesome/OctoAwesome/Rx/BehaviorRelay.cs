using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    /// <summary>
    /// Represents a value that changes over time.
    /// Observers can subscribe to the subject to receive the last (or initial) value and all subsequent notifications.
    /// </summary>
    /// <typeparam name="T">The type of the elements processed by the subject.</typeparam>
    /// <seealso cref="Relay{T}"/>
    public class BehaviorRelay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly List<BehaviorRelaySubscription> subscriptions;

        private T lastValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorRelay{T}"/> class.
        /// </summary>
        public BehaviorRelay(T initialValue)
        {
            subscriptions = new();
            lastValue = initialValue;
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnCompleted();
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnError(error);
            }
        }

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            lastValue = value;
            for (int i = 0; i < subscriptions.Count; i++)
            {
                subscriptions[i]?.Observer.OnNext(value);
            }
        }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new BehaviorRelaySubscription(this, observer);
            subscriptions.Add(sub);
            sub.Observer.OnNext(lastValue);
            return sub;
        }

        /// <inheritdoc/>
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

            /// <inheritdoc/>
            public void Dispose()
            {
                relay.Unsubscribe(this);
            }
        }
    }
}
