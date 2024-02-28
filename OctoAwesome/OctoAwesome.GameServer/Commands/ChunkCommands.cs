
using OctoAwesome.Chunking;
using OctoAwesome.Location;
using OctoAwesome.Network;
using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.GameServer.Commands
{
    /// <summary>
    /// Contains commands for chunk loading and saving remotely.
    /// </summary>
    public static class ChunkCommands
    {
        /// <summary>
        /// Loads column data from <see cref="CommandParameter"/> given location.
        /// </summary>
        /// <param name="parameter">The <see cref="CommandParameter"/> given location to load the column at.</param>
        /// <returns>The loaded chunk column data.</returns>
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

            return Serializer.Serialize(column);
        }

        /// <summary>
        /// Saves chunk column data received from <see cref="CommandParameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="CommandParameter"/> containing the chunk column data.</param>
        /// <returns><c>null</c></returns>
        public static byte[]? SaveColumn(CommandParameter parameter)
        {
            var chunkColumn = Serializer.Deserialize<ChunkColumn>(parameter.Data);

            TypeContainer.Get<SimulationManager>().Simulation.ResourceManager.SaveChunkColumn(chunkColumn);

            return null;
        }
    }
}
