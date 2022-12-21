using OctoAwesome.Rx;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Update hub implementation for managing observers and observables in a thread safe manner.
    /// </summary>
    public class UpdateHub : IDisposable, IUpdateHub
    {
        private readonly Dictionary<string, ConcurrentRelay<object>> channels;
        private readonly LockSemaphore lockSemaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateHub"/> class.
        /// </summary>
        public UpdateHub()
        {
            channels = new();
            lockSemaphore = new LockSemaphore(1, 1);
        }

        /// <inheritdoc />
        public IObservable<object> ListenOn(string channel)
            => GetChannelRelay(channel);

        /// <inheritdoc />
        public IDisposable AddSource(IObservable<object> notification, string channel)
            => notification.Subscribe(GetChannelRelay(channel));

        private ConcurrentRelay<object> GetChannelRelay(string channel)
        {
            using var scope = lockSemaphore.Wait();

            if (!channels.TryGetValue(channel, out var channelRelay))
            {
                channelRelay = new();
                channels.Add(channel, channelRelay);
            }

            return channelRelay;
        }

        /// <inheritdoc />
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
