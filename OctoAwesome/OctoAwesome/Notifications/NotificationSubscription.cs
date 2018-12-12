using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public sealed class NotificationSubscription : IDisposable
    {
        private IUpdateProvider updateProvider;
        private IObserver<Notification> subscriber;

        public NotificationSubscription(IUpdateProvider updateProvider, IObserver<Notification> subscriber)
        {
            this.subscriber = subscriber;
            this.updateProvider = updateProvider;
        }

        public void Dispose()
        {
            updateProvider.Unsubscribe(subscriber);
            updateProvider = null;

            subscriber = null;
        }
    }
}
