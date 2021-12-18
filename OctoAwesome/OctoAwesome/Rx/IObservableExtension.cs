using System;

namespace OctoAwesome.Rx
{
    /// <summary>
    /// Extensions for <see cref="IObservable{T}"/>.
    /// </summary>
    public static class IObservableExtension
    {
        /// <summary>
        /// Notifies the provider that an observer action is to receive notifications.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action that provides the observer with new data.</param>
        /// <typeparam name="T">The object that provides notification information.</typeparam>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications
        /// before the provider has finished sending them.
        /// </returns>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
            => observable.Subscribe(new Observer<T>(onNext));

        /// <summary>
        /// Notifies the provider that observer actions is to receive notifications.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action that provides the observer with new data.</param>
        /// <param name="onException">
        /// Action to notify the observer that the provider has experienced an error condition.
        /// </param>
        /// <typeparam name="T">The object that provides notification information.</typeparam>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications
        /// before the provider has finished sending them.
        /// </returns>
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException)
            => observable.Subscribe(new Observer<T>(onNext, onException));

        /// <summary>
        /// Notifies the provider that observer actions is to receive notifications.
        /// </summary>
        /// <param name="observable">The observable to subscribe to.</param>
        /// <param name="onNext">Action that provides the observer with new data.</param>
        /// <param name="onException">
        /// Action to notify the observer that the provider has experienced an error condition.
        /// </param>
        /// <param name="onComplete">
        /// Action to notify the observer that the provider has finished sending push-based notifications.
        /// </param>
        /// <typeparam name="T">The object that provides notification information.</typeparam>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications
        /// before the provider has finished sending them.
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
