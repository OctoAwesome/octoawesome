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
        [Command((ushort)12)]
        public static byte[] GetUniverse(byte[] data)
        {
            var universe = Program.ServerHandler.SimulationManager.GetUniverse();
            
            using (var memoryStream = new MemoryStream())
            {                
                universe.Serialize(memoryStream);
                return memoryStream.ToArray();
            }
        }

        [Command((ushort)13)]
        public static byte[] GetPlanet(byte[] data)
        {
            //var universe = Program.ServerHandler.SimulationManager.GetPlanet();

            //using (var memoryStream = new MemoryStream())
            //{
            //    universe.Serialize(memoryStream);
            //    return memoryStream.ToArray();
            //}
            throw new NotImplementedException();
        }
    }
}
