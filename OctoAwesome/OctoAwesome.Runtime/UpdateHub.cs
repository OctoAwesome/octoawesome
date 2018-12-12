using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateProvider, IUpdateHub, IDisposable
    {
        private readonly HashSet<IObserver<Notification>> observers;
        private readonly SemaphoreSlim observerSemaphore;

        public UpdateHub()
        {
            observers = new HashSet<IObserver<Notification>>();
            observerSemaphore = new SemaphoreSlim(0, 1);
        }

        public IDisposable Subscribe(IObserver<Notification> observer)
        {
            observerSemaphore.Wait();
            observers.Add(observer);
            observerSemaphore.Release();

            return new NotificationSubscription(this, observer);
        }

        public void Unsubscribe(IObserver<Notification> subscriber)
        {
            observerSemaphore.Wait();
            observers.Remove(subscriber);
            observerSemaphore.Release();
        }

        public void Push(Notification notification)
        {
            observerSemaphore.Wait();

            foreach (var observer in observers)
                observer.OnNext(notification);

            observerSemaphore.Release();
        }

        public void Dispose()
        {
            observerSemaphore.Wait();

            foreach (var observer in observers)
                observer.OnCompleted();

            observers.Clear();
            observerSemaphore.Release();            
        }
    }
}
