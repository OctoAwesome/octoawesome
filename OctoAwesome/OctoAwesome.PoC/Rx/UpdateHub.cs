using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;

namespace OctoAwesome.PoC.Rx
{
    public class UpdateHub : IDisposable
    {
        private readonly Dictionary<string, Relay<Notification>> channels;

        public UpdateHub()
        {
            channels = new();
        }

        public IObservable<Notification> ListenOn(string channel)
            => GetChannelRelay(channel);

        public IDisposable AddSource(IObservable<Notification> notification, string channel) 
            => notification.Subscribe(GetChannelRelay(channel));

        private Relay<Notification> GetChannelRelay(string channel)
        {
            if (!channels.TryGetValue(channel, out var channelRelay))
            {
                channelRelay = new();
                channels.Add(channel, channelRelay);
            }

            return channelRelay;
        }

        public void Dispose()
        {
            foreach (var channel in channels)
            {
                channel.Value.Dispose();
            }

            channels.Clear();
        }

    }
}
