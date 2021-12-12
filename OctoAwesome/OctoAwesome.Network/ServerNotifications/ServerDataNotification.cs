using OctoAwesome.Notifications;
using System.Collections.Generic;

namespace OctoAwesome.Network.ServerNotifications
{
    public class ServerDataNotification : Notification
    {
        public byte[] Data { get; set; }

        public OfficialCommand OfficialCommand { get; set; }
        
        public HashSet<int> PlayerIds { get; }
        
        public ServerDataNotification()
        {
            PlayerIds = new HashSet<int>();
        }
        public override bool Match<T>(T filter) 
        {
            if (PlayerIds.Count < 1)
                return true;

            if (filter is int playerId)
                return PlayerIds.Contains(playerId);
            
            return false;
        }
    }
}
