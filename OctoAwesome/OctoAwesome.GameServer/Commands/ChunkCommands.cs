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
    public class ChunkCommands
    {
        [Command((ushort)OfficialCommand.LoadColumn)]
        public static byte[] LoadColumn(CommandParameter parameter)
        {
            Guid guid;
            int planetId;
            Index2 index2;

            using (var memoryStream = new MemoryStream(parameter.Data))
            using (var reader = new BinaryReader(memoryStream))
            {
                guid = new Guid(reader.ReadBytes(16));
                planetId = reader.ReadInt32();
                index2 = new Index2(reader.ReadInt32(), reader.ReadInt32());
            }
            var column = Program.ServerHandler.SimulationManager.LoadColumn(guid, planetId, index2);

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream))
            {
                column.Serialize(writer);
                return memoryStream.ToArray();
            }
        }

        [Command((ushort)OfficialCommand.SaveColumn)]
        public static byte[] SaveColumn(CommandParameter parameter)
        {
            var chunkColumn = new ChunkColumn();

            using (var memoryStream = new MemoryStream(parameter.Data))
            using (var reader = new BinaryReader(memoryStream))
            {
                chunkColumn.Deserialize(reader);
            }

            Program.ServerHandler.SimulationManager.Simulation.ResourceManager.SaveChunkColumn(chunkColumn);

            return null;
        }
    }
}
