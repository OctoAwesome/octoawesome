using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer.Commands
{
    public static class GeneralCommands
    {
        [Command((ushort)OfficialCommand.GetUniverse)]
        public static byte[] GetUniverse(CommandParameter parameter)
        {
            var universe = Program.ServerHandler.SimulationManager.GetUniverse();
            
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                universe.Serialize(writer);
                return memoryStream.ToArray();
            }
        }

        [Command((ushort)OfficialCommand.GetPlanet)]
        public static byte[] GetPlanet(CommandParameter parameter)
        {
            var planet = Program.ServerHandler.SimulationManager.GetPlanet(0);

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                planet.Serialize(writer);
                return memoryStream.ToArray();
            }
        }
    }
}
