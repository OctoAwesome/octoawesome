using OctoAwesome.Rx;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    public record struct PushInfo(object Notification, string Channel);

    /// <summary>
    /// Update hub implementation for managing observers and observables in a thread safe manner.
    /// </summary>
    public class UpdateHub : IDisposable, IUpdateHub
    {
        private readonly Dictionary<string, ConcurrentRelay<object>> channels;
        private readonly ConcurrentRelay<PushInfo> networkChannel;
        private readonly LockSemaphore lockSemaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHub"/> class.
        /// </summary>
        public UpdateHub()
        {
            channels = new();
            lockSemaphore = new LockSemaphore(1, 1);
            networkChannel = new ConcurrentRelay<PushInfo>();
        }

        /// <inheritdoc />
        public IObservable<object> ListenOn(string channel)
            => GetChannelRelay(channel);

        /// <inheritdoc />
        public IObservable<PushInfo> ListenOnNetwork() => networkChannel;

        /// <inheritdoc />
        public IDisposable AddSource(IObservable<object> notification, string channel, bool sendOverNetwork = false)
        {
            if (sendOverNetwork)
            {
                return StableCompositeDisposable.Create(
                    notification.Subscribe(GetChannelRelay(channel)),
                    notification.Subscribe(x => networkChannel.OnNext(new(x, channel))));
            }
            else
            {
                return notification.Subscribe(GetChannelRelay(channel));
            }
        }


        public ConcurrentRelay<object> GetChannelRelay(string channel)
        {
            using var scope = lockSemaphore.Wait();

            if (!channels.TryGetValue(channel, out var channelRelay))
            {
                channelRelay = new();
                channels.Add(channel, channelRelay);
            }

            return channelRelay;
        }


        public void PushNetwork(object notification, string channel)
        {
            networkChannel.OnNext(new PushInfo(notification, channel));
        }

        public void Push(object notification, string channel)
        {
            if (channels.TryGetValue(channel, out var channelRelay))
                channelRelay.OnNext(notification);
        }

        public void Dispose()
        {
            foreach (var channel in channels)
            {
                channel.Value.Dispose();
            }

            channels.Clear();
            lockSemaphore.Dispose();
            networkChannel?.Dispose();
        }
    }

}
