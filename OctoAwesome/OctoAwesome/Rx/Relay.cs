using NonSucking.Framework.Extension.Collections;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    /// <summary>
    /// Class for relaying observed data to multiple observers.
    /// </summary>
    /// <typeparam name="T">The type of the observable and observed data.</typeparam>
    public class Relay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly EnumerationModifiableList<RelaySubscription> subscriptions;
        private readonly EnumerationModifiableList<RelaySubscription> oneShotSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Relay{T}"/> class.
        /// </summary>
        public Relay()
        {
            subscriptions = new();
            oneShotSubscriptions = new();
        }

        /// <inheritdoc />
        public void OnCompleted()
        {
            foreach (RelaySubscription sub in subscriptions)
            {
                sub.Observer.OnCompleted();
            }
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            foreach (RelaySubscription sub in subscriptions)
            {
                sub.Observer.OnError(error);
            }
        }

        /// <inheritdoc />
        public void OnNext(T value)
        {
            foreach (RelaySubscription sub in subscriptions)
            {
                sub.Observer.OnNext(value);
            }

            foreach (RelaySubscription sub in oneShotSubscriptions)
            {
                sub.Observer.OnNext(value);
                sub.Dispose();
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(subscriptions, observer);
            subscriptions.Add(sub);
            return sub;
        }
        /// <inheritdoc />
        public void SubscribeOnce(IObserver<T> observer)
        {
#pragma warning disable DF0010 // Marks undisposed local variables.
            var sub = new RelaySubscription(oneShotSubscriptions, observer);
#pragma warning restore DF0010 // Marks undisposed local variables.
            oneShotSubscriptions.Add(sub);
        }

        /// <inheritdoc />
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

            private readonly IList<RelaySubscription> parent;

            public RelaySubscription(IList<RelaySubscription> parent,IObserver<T> observer)
            {
                Observer = observer;
                this.parent = parent;
            }

            public void Dispose()
            {
                parent.Remove(this);
            }
        }
    }
}
