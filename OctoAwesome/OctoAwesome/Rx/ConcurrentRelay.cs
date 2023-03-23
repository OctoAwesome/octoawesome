using NonSucking.Framework.Extension.Collections;

using OctoAwesome.Logging;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Rx
{
    /// <summary>
    /// Class for thread safely relaying observed data to multiple observers.
    /// </summary>
    /// <typeparam name="T">The type of the observable and observed data.</typeparam>
    /// <seealso cref="Relay{T}"/>
    public class ConcurrentRelay<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private readonly EnumerationModifiableConcurrentList<RelaySubscription> subscriptions;
        private readonly ILogger logger;
        private readonly LockSemaphore lockSemaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentRelay{T}"/> class.
        /// </summary>
        public ConcurrentRelay()
        {
            lockSemaphore = new LockSemaphore(1, 1);
            subscriptions = new();
            logger = TypeContainer.Get<ILogger>().As(nameof(ConcurrentRelay<T>));
        }

        /// <inheritdoc />
        public void OnCompleted()
        {
            using var scope = lockSemaphore.Wait();

            foreach (var sub in subscriptions)
            {
                sub.Observer.OnCompleted();
            }
        }

        /// <inheritdoc />
        public void OnError(Exception error)
        {
            using var scope = lockSemaphore.Wait();

            foreach (var sub in subscriptions)
            {
                sub.Observer.OnError(error);
            }
        }

        /// <inheritdoc />
        public void OnNext(T value)
        {
            using var scope = lockSemaphore.Wait();
            logger.Trace($"Got lock, dispatching {value} to {subscriptions.Count} subs");
            
            foreach (var sub in subscriptions)
            {
                sub.Observer.OnNext(value);
            }
        }

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var sub = new RelaySubscription(this, observer);

            subscriptions.Add(sub);

            return sub;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            subscriptions.Clear();
            lockSemaphore.Dispose();
        }

        private void Unsubscribe(RelaySubscription subscription)
        {
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
