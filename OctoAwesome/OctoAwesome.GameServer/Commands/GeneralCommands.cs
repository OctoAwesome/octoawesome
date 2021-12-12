using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using System;
using System.IO;

namespace OctoAwesome.GameServer.Commands
{
    public static class GeneralCommands
    {
        [Command((ushort)OfficialCommand.GetUniverse)]
        public static byte[] GetUniverse(CommandParameter parameter) // TODO: use parameter for multi universe server?
        {
            var universe = TypeContainer.Get<SimulationManager>().GetUniverse();

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                universe.Serialize(writer);
                return memoryStream.ToArray();
            }
        }
        [Command((ushort)OfficialCommand.GetPlanet)]
        public static byte[] GetPlanet(CommandParameter parameter) // TODO: use parameter for actual planet server?
        {
            Console.WriteLine("Just got in here");

            var planet = TypeContainer.Get<SimulationManager>().GetPlanet(0);

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                planet.Serialize(writer);
                Console.WriteLine("Sending Planet Result");
                return memoryStream.ToArray();
            }
        }
    }
}
