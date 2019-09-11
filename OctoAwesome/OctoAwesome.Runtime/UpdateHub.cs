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

        public UpdateHub()
            => observers = new NotificationChannelCollection();

        public IDisposable Subscribe(INotificationObserver observer, string channel = "none")
        {
            observers.Add(channel, observer);
            return new NotificationSubscription(this, observer, channel);
        }

        public void Unsubscribe(INotificationObserver observer)
            => observers.Remove(observer);

        public void Unsubscribe(INotificationObserver observer, string channel)
            => observers.Remove(channel, observer);

        public void Push(Notification notification)
        {
            foreach (KeyValuePair<string, ObserverHashSet> observerSet in observers)
            {
                using (observerSet.Value.Wait())
                {
                    foreach (INotificationObserver observer in observerSet.Value)
                        observer.OnNext(notification);
                }
            }
        }
        public void Push(Notification notification, string channel)
        {

            if (observers.TryGetValue(channel, out ObserverHashSet observerSet))
            {
                using (observerSet.Wait())
                {
                    foreach (INotificationObserver observer in observerSet)
                        observer.OnNext(notification);
                }
            }

        }

        public void Dispose()
        {

            foreach (KeyValuePair<string, ObserverHashSet> observerSet in observers)
            {
                using (observerSet.Value.Wait())
                {
                    foreach (INotificationObserver observer in observerSet.Value)
                        observer.OnCompleted();
                }
            }
            observers.Clear();
        }

    }
}
