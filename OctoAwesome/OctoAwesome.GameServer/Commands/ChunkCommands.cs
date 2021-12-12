using CommandManagementSystem.Attributes;
using OctoAwesome.Network;
using System;
using System.IO;

namespace OctoAwesome.GameServer.Commands
{
    public static class ChunkCommands
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

            var column = TypeContainer.Get<SimulationManager>().LoadColumn(planetId, index2);

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
            var chunkColumn = new ChunkColumn(null);

            using (var memoryStream = new MemoryStream(parameter.Data))
            using (var reader = new BinaryReader(memoryStream))
            {
                chunkColumn.Deserialize(reader);
            }

            TypeContainer.Get<SimulationManager>().Simulation.ResourceManager.SaveChunkColumn(chunkColumn);

            return null;
        }
    }
}
