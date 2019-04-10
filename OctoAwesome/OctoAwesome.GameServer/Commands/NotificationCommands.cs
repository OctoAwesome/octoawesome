using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
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

        static NotificationCommands()
        {
            updateHub = Program.ServerHandler.UpdateHub;
        }

        [Command((ushort)OfficialCommand.EntityNotification)]
        public static byte[] EntityNotification(CommandParameter parameter)
        {
            var entityNotification = Serializer.Deserialize<EntityNotification>(parameter.Data, null);
            entityNotification.SenderId = parameter.ClientId;
            updateHub.Push(entityNotification, DefaultChannels.Simulation);
            updateHub.Push(entityNotification, DefaultChannels.Network);
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(CommandParameter parameter)
        {
            var chunkNotification = Serializer.Deserialize<ChunkNotification>(parameter.Data, null);
            chunkNotification.SenderId = parameter.ClientId;
            updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            updateHub.Push(chunkNotification, DefaultChannels.Network);
            return null;
        }
    }
}
