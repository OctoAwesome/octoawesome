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
        public static byte[] EntityNotification(byte[] data)
        {
            var entityNotification = Serializer.Deserialize<EntityNotification>(data, null);
            updateHub.Push(entityNotification, DefaultChannels.Simulation);
            updateHub.Push(entityNotification, DefaultChannels.Network);
            return null;
        }

        [Command((ushort)OfficialCommand.ChunkNotification)]
        public static byte[] ChunkNotification(byte[] data)
        {
            var chunkNotification = Serializer.Deserialize<ChunkNotification>(data, null);
            updateHub.Push(chunkNotification, DefaultChannels.Chunk);
            updateHub.Push(chunkNotification, DefaultChannels.Network);
            return null;
        }
    }
}
