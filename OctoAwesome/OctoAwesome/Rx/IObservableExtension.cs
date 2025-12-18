using System;

namespace OctoAwesome.Rx
{
    /// <summary>
    /// Extensions for <see cref="IObservable{T}"/>.
    /// </summary>
    public static class IObservableExtension
    {
        /// <summary>
        /// Subscribes an element handler to an observable sequence.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <typeparam name="T">The object type that provides notification information.</typeparam>
        /// <returns>
        /// <see cref="IDisposable"/> object used to unsubscribe from the observable sequence.
        /// </returns>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
            => observable.Subscribe(new Observer<T>(onNext));

        /// <summary>
        /// Subscribes an element handler to an observable sequence and disposes after once onNext.
        /// </summary>
        /// <param name="observable">The relay to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <typeparam name="T">The object type that provides notification information.</typeparam>

        public static void SubscribeOnce<T>(this Relay<T> observable, Action<T> onNext)
            => observable.SubscribeOnce(new Observer<T>(onNext));
        

        /// <summary>
        /// Subscribes an element handler and an exception handler handler to an observable sequence.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onException">
        /// Action to invoke upon exceptional termination of the observable sequence.
        /// </param>
        /// <typeparam name="T">The object type that provides notification information.</typeparam>
        /// <returns>
        /// <see cref="IDisposable"/> object used to unsubscribe from the observable sequence.
        /// </returns>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException)
            => observable.Subscribe(new Observer<T>(onNext, onException));

        /// <summary>
        /// Subscribes an element handler, an exception handler, and a completion handler to an observable sequence.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action to invoke for each element in the observable sequence.</param>
        /// <param name="onException">
        /// Action to invoke upon exceptional termination of the observable sequence.
        /// </param>
        /// <param name="onComplete">
        /// Action to invoke upon graceful termination of the observable sequence.
        /// </param>
        /// <typeparam name="T">The object type that provides notification information.</typeparam>
        /// <returns>
        /// <see cref="IDisposable"/> object used to unsubscribe from the observable sequence.
        /// </returns>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException, Action onComplete)
            => observable.Subscribe(new Observer<T>(onNext, onException, onComplete));


        private class Observer<T> : IObserver<T>
        {
            private readonly Action<T>? onNext;
            private readonly Action<Exception>? onException;
            private readonly Action? onComplete;

            public Observer(Action<T>? onNext = null, Action<Exception>? onException = null, Action? onComplete = null)
            {
                this.onNext = onNext;
                this.onException = onException;
                this.onComplete = onComplete;
            }

            public void OnCompleted()
                => onComplete?.Invoke();

            public void OnError(Exception error)
                => onException?.Invoke(error);

            public void OnNext(T value)
                => onNext?.Invoke(value);
        }
    }
}
