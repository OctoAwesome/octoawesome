using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;

namespace OctoAwesome.GameServer.Commands
{

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
        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, parameter.Data);
            entityNotification.SenderId = parameter.ClientId;

            simulationChannel.OnNext(entityNotification);
            networkChannel.OnNext(entityNotification);

            entityNotification.Release();
            return null;
        }
        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
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

            return null;
        }
    }
}
