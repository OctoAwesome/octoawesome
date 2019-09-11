using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public sealed class NotificationSubscription : IDisposable
    {
        private INotificationObservable observable;
        private INotificationObserver observer;
        private readonly string channel;

        public NotificationSubscription(INotificationObservable observable, INotificationObserver observer, string channel)
        {
            this.observer = observer;
            this.observable = observable;
            this.channel = channel;
        }

        public void Dispose()
        {
            observer.OnCompleted();
            observable.Unsubscribe(observer, channel);
            observable = null;
            observer = null;
        }
    }
}
