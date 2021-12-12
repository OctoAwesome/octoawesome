using OctoAwesome.Rx;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{

    public class UpdateHub : IDisposable, IUpdateHub
    {
        private readonly Dictionary<string, ConcurrentRelay<Notification>> channels;
        private readonly LockSemaphore lockSemaphore;
        public UpdateHub()
        {
            channels = new();
            lockSemaphore = new LockSemaphore(1, 1);
        }
        public IObservable<Notification> ListenOn(string channel)
            => GetChannelRelay(channel);
        public IDisposable AddSource(IObservable<Notification> notification, string channel) 
            => notification.Subscribe(GetChannelRelay(channel));

        private ConcurrentRelay<Notification> GetChannelRelay(string channel)
        {
            using var scope = lockSemaphore.Wait();

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
            lockSemaphore.Dispose();
        }

    }
}
