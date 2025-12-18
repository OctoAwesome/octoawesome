
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.Network.Commands
{
    /// <summary>
    /// Contains remote notification commands.
    /// </summary>
    public class NotificationCommands : IDisposable
    {
        private readonly IUpdateHub updateHub;
        private readonly IDisposable chunkChannelSubscription;
        private readonly IDisposable chatChannelSubscription;

        public NotificationCommands()
        {
            updateHub = TypeContainer.Get<IUpdateHub>();

            chunkChannelSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(ChunkChannelOnNext);
            chatChannelSubscription = updateHub.ListenOn(DefaultChannels.Chat).Subscribe(OnChatMessageReceived);
        }

        private void OnChatMessageReceived(object obj)
        {
            updateHub.PushNetwork(obj, DefaultChannels.Chat);
        }

        private void ChunkChannelOnNext(object obj)
        {
            updateHub.PushNetwork(obj, DefaultChannels.Chunk);
        }

        public void Dispose()
        {
            chunkChannelSubscription.Dispose();
            chatChannelSubscription.Dispose();
        }
    }
}
