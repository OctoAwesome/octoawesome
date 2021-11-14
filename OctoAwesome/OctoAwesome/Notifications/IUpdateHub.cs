using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface IUpdateHub 
    {
        IDisposable AddSource(IObservable<Notification> notification, string channel);
        IObservable<Notification> ListenOn(string channel);
    }
}
