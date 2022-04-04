using System;

namespace OctoAwesome.Notifications
{

    public interface IUpdateHub
    {

        IDisposable AddSource(IObservable<Notification> notification, string channel);
        IObservable<Notification> ListenOn(string channel);
    }
}
