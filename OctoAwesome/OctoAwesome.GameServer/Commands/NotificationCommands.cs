using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer.Commands
{
    public static class NotificationCommands
    {
        private static readonly IUpdateHub updateHub;
        private static readonly IPool<EntityNotification> entityNotificationPool;
        private static readonly IPool<ChunkNotification> chunkNotificationPool;

        static NotificationCommands()
        {
            updateHub = TypeContainer.Get<IUpdateHub>();
            entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            chunkNotificationPool = TypeContainer.Get<IPool<ChunkNotification>>();
        }

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.DeserializePoolElement(entityNotificationPool, parameter.Data);
            entityNotification.SenderId = parameter.ClientId;
            updateHub.Push(entityNotification, DefaultChannels.Simulation);
            updateHub.Push(entityNotification, DefaultChannels.Network);
            entityNotification.Release();
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var chunkNotification = Serializer.DeserializePoolElement(chunkNotificationPool, parameter.Data);
            chunkNotification.SenderId = parameter.ClientId;
            updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            updateHub.Push(chunkNotification, DefaultChannels.Network);
            chunkNotification.Release();

            return null;
        }
    }
}
