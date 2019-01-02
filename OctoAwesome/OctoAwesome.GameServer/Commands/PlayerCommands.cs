using CommandManagementSystem.Attributes;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using OctoAwesome.Network.ServerNotifications;
using OctoAwesome.Notifications;
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
        private static IUpdateHub updateHub;

        static PlayerCommands()
        {
            updateHub = Program.ServerHandler.UpdateHub;
        }


        [Command((ushort)OfficialCommand.Whoami)]
        public static byte[] Whoami(byte[] data)
        {
            string playername = Encoding.UTF8.GetString(data);
            var player = new RemoteEntity(new Player());

            player.Components.AddComponent(new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) });
            player.Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                player.Serialize(bw, Program.ServerHandler.SimulationManager.DefinitionManager);
                Console.WriteLine(playername);
                var array = ms.ToArray();
                updateHub.Push(new ServerDataNotification()
                {
                    Data = array,
                    OfficialCommand = OfficialCommand.NewEntity
                });

                return array;
            }
        }
    }
}
