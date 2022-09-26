using OctoAwesome.Notifications;
using System.Collections.Generic;
using OctoAwesome.Extension;

namespace OctoAwesome.Network.ServerNotifications
{
    /// <summary>
    /// Server data notification.
    /// </summary>
    public class ServerDataNotification : Notification
    {
        private byte[]? data;

        /// <summary>
        /// Gets or sets the notification data.
        /// </summary>
        public byte[] Data
        {
            get => NullabilityHelper.NotNullAssert(data);
            set => data = NullabilityHelper.NotNullAssert(value);
        }

        /// <summary>
        /// Gets or sets the notification command.
        /// </summary>
        public OfficialCommand OfficialCommand { get; set; }

        /// <summary>
        /// Gets a set of player ids this notification is meant for.
        /// </summary>
        public HashSet<int> PlayerIds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerDataNotification"/> class.
        /// </summary>
        public ServerDataNotification()
        {
            PlayerIds = new HashSet<int>();
        }

        /// <inheritdoc />
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
