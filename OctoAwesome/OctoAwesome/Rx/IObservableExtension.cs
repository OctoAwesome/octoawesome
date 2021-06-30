using System;

namespace OctoAwesome.Rx
{
    public static class IObservableExtension
    {
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext)
            => observable.Subscribe(new Observer<T>(onNext));
        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onException, Action onComplete) 
            => observable.Subscribe(new Observer<T>(onNext, onException, onComplete));

        private class Observer<T> : IObserver<T>
        {
            private readonly Action<T> onNext;
            private readonly Action<Exception> onException;
            private readonly Action onComplete;

            public Observer(Action<T> onNext = null, Action<Exception> onException = null, Action onComplete = null)
            {
                this.onNext = onNext;
                this.onException = onException;
                this.onComplete = onComplete ;
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
