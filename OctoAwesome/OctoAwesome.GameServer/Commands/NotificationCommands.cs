
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
    public static class NotificationCommands
    {
        private static readonly IPool<EntityNotification> entityNotificationPool;
        private static readonly IPool<BlockChangedNotification> blockChangedNotificationPool;
        private static readonly IPool<BlocksChangedNotification> blocksChangedNotificationPool;

        private static readonly ConcurrentRelay<Notification> simulationChannel;
        private static readonly ConcurrentRelay<Notification> networkChannel;
        private static readonly ConcurrentRelay<Notification> chunkChannel;
        private static readonly IDisposable simulationChannelSub;
        private static readonly IDisposable networkChannelSub;
        private static readonly IDisposable chunkChannelSub;

        static NotificationCommands()
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            blocksChangedNotificationPool = TypeContainer.Get<IPool<BlocksChangedNotification>>();

            simulationChannel = new();
            networkChannel = new();
            chunkChannel = new();

            simulationChannelSub = updateHub.AddSource(simulationChannel, DefaultChannels.Simulation);
            networkChannelSub = updateHub.AddSource(networkChannel, DefaultChannels.Network);
            chunkChannelSub = updateHub.AddSource(chunkChannel, DefaultChannels.Chunk);
        }

        /// <summary>
        /// Manifests entity changes received from <see cref="CommandParameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="CommandParameter"/> containing the entity notification data.</param>
        /// <returns><c>null</c></returns>
        public static void EntityNotification(ITypeContainer tc, CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, parameter.Data);

            entityNotification.SenderId = parameter.ClientId;

            simulationChannel.OnNext(entityNotification);
            networkChannel.OnNext(entityNotification);

            entityNotification.Release();
        }

        /// <summary>
        /// Manifests block changes received from <see cref="CommandParameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="CommandParameter"/> containing the chunk notification data.</param>
        /// <returns><c>null</c></returns>
        public static void ChunkNotification(ITypeContainer tc, CommandParameter parameter)
        {
            var notificationType = (BlockNotificationType)parameter.Data[0];
            Notification chunkNotification;
            switch (notificationType)
            {
                case BlockNotificationType.BlockChanged:
                    chunkNotification = Serializer.DeserializePoolElement(blockChangedNotificationPool, parameter.Data);
                    break;
                case BlockNotificationType.BlocksChanged:
                    chunkNotification = Serializer.DeserializePoolElement(blocksChangedNotificationPool, parameter.Data);
                    break;
                default:
                    throw new NotSupportedException($"This Type is not supported: {notificationType}");
            }

            chunkNotification.SenderId = parameter.ClientId;

            chunkChannel.OnNext(chunkNotification);
            networkChannel.OnNext(chunkNotification);

            chunkNotification.Release();

        }
    }
}
