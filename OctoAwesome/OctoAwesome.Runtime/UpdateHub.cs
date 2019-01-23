using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public class UpdateHub : IUpdateHub, IDisposable
    {
        private readonly NotificationChannelCollection observers;
        private readonly SemaphoreSlim observerSemaphore;

        public UpdateHub()
        {
            observers = new NotificationChannelCollection();
            observerSemaphore = new SemaphoreSlim(1, 1);
        }

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            observerSemaphore.Wait();
            observers.Add(channel, observer);
            observerSemaphore.Release();

            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer)
        {
            observerSemaphore.Wait();
            observers.Remove(observer);
            observerSemaphore.Release();
        }

        public void Unsubscribe(INotificationObserver observer, string channel)
        {
            observerSemaphore.Wait();
            observers.Remove(channel, observer);
            observerSemaphore.Release();
        }

        public void Push(Notification notification)
        {
            observerSemaphore.Wait();

            foreach (var observerSet in observers)
                foreach (var observer in observerSet.Value)
                    observer.OnNext(notification);

            observerSemaphore.Release();
        }
        public void Push(Notification notification, string channel)
        {
            observerSemaphore.Wait();

            if (observers.TryGetValue(channel, out var observerSet))
            {
                foreach (var observer in observerSet)
                    observer.OnNext(notification);
            }

            observerSemaphore.Release();
        }

        public void Dispose()
        {
            observerSemaphore.Wait();

            foreach (var observerSet in observers)
                foreach (var observer in observerSet.Value)
                    observer.OnCompleted();

            observers.Clear();
            observerSemaphore.Release();
        }


    }
}
