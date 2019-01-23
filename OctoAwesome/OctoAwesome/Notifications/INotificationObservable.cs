using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface INotificationObservable 
    {
        IDisposable Subscribe(INotificationObserver observer, string channel = "none");
        void Unsubscribe(INotificationObserver observer, string channel);
        void Unsubscribe(INotificationObserver observer);

    }
}
