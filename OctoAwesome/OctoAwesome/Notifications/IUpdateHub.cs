using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface IUpdateHub : INotificationObservable
    {
        void Push(Notification notification, string channel);
        void Push(Notification notification);
    }
}
