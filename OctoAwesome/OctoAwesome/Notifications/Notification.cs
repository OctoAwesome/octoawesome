using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public abstract class Notification
    {
        public uint SenderId { get; set; }

        public virtual bool Match<T>(T filter)
        {
            return true;
        }
    }
}
