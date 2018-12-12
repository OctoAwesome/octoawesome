using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface IUpdateProvider : IObservable<Notification>
    {
        void Unsubscribe(IObserver<Notification> subscriber);
        
    }
}
