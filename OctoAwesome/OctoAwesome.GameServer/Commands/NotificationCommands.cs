
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.GameServer.Commands
{
    /// <summary>
    /// Contains remote notification commands.
    /// </summary>
    public class NotificationCommands : IDisposable
    {
        private readonly IUpdateHub updateHub;
        private readonly IDisposable chunkChannelSubscription;

        public NotificationCommands()
        {
            updateHub = TypeContainer.Get<IUpdateHub>();

            chunkChannelSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(ChunkChannelOnNext);
        }

        private void ChunkChannelOnNext(object obj)
        {
            updateHub.PushNetwork(obj, DefaultChannels.Chunk);
        }
        public void Dispose()
        {
            chunkChannelSubscription.Dispose();
        }
    }
}
