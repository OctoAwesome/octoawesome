using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class PropertyChangedNotification : Notification
    {
        public string Issuer { get; set; }
        public string Property { get; set; }

        public byte[] Value { get; set; }
    }
}
