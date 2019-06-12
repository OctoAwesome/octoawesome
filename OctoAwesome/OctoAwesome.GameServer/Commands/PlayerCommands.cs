using CommandManagementSystem.Attributes;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using OctoAwesome.Network.ServerNotifications;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
      

        [Command((ushort)OfficialCommand.Whoami)]
        public static byte[] Whoami(CommandParameter parameter)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            string playername = Encoding.UTF8.GetString(parameter.Data);
            var player = new Player();

            updateHub.Push(new EntityNotification()
            {
                Entity = player,
                Type = EntityNotification.ActionType.Add
            }, DefaultChannels.Simulation);

            var remotePlayer = new RemoteEntity(player);
            remotePlayer.Components.AddComponent(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });
            remotePlayer.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            remotePlayer.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });

            Console.WriteLine(playername);

            updateHub.Push(new EntityNotification()
            {
                Entity = remotePlayer,
                Type = EntityNotification.ActionType.Add
            }, DefaultChannels.Network);


            return Serializer.Serialize(player);
        }
    }
}
