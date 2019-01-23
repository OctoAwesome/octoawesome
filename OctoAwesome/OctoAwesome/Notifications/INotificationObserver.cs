using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface INotificationObserver
    {
        void OnCompleted();
        void OnError(Exception error);
        void OnNext(Notification value);
    }
}
