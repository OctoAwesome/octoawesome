using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Serialisierer für Chunks.
    /// </summary>
    public class ChunkSerializer : IChunkSerializer
    {
        /// <summary>
        /// Deserialisiert einen Chunk.
        /// </summary>
        /// <param name="stream">Der Stream mit Chunk-Daten.</param>
        /// <param name="position">Die Position des Chunks.</param>
        /// <returns>Der deserialisierte Chunk.</returns>
        public IChunk Deserialize(Stream stream, PlanetIndex3 position)
        {
            Chunk chunk = new Chunk(position.ChunkIndex, position.Planet);

            using (BinaryReader br = new BinaryReader(stream))
            {
                List<IBlockDefinition> types = new List<IBlockDefinition>();
                Dictionary<ushort, ushort> map = new Dictionary<ushort, ushort>();

                bool longIndex = br.ReadByte() > 0;

                int typecount = longIndex ? br.ReadUInt16() : br.ReadByte();

                // Im Falle eines Luftchunks
                if (typecount == 0)
                    return chunk;

                for (int i = 0; i < typecount; i++)
                {
                    string typeName = br.ReadString();
                    IBlockDefinition[] definitions = DefinitionManager.GetBlockDefinitions().ToArray();
                    var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                    types.Add(blockDefinition);

                    map.Add((ushort)types.Count, (ushort)(Array.IndexOf(definitions, blockDefinition) + 1));
                }

                for (int i = 0; i < chunk.Blocks.Length; i++)
                {
                    ushort typeIndex = longIndex ? br.ReadUInt16() : br.ReadByte();
                    chunk.MetaData[i] = 0;
                    if (typeIndex > 0)
                    {
                        chunk.Blocks[i] = map[typeIndex];

                        var definition = DefinitionManager.GetBlockDefinitionByIndex(map[typeIndex]);
                        if (definition.HasMetaData)
                            chunk.MetaData[i] = br.ReadInt32();
                    }
                }
            }

            return chunk;
        }

        /// <summary>
        /// Serialisiert einen Chunk.
        /// </summary>
        /// <param name="stream">Der Stream, in den die Chunk-Daten geschrieben werden sollen (wird nach dem Schreiben geschlossen).</param>
        /// <param name="chunk">Der zu serialisierende Chunk.</param>
        public void Serialize(Stream stream, IChunk chunk)
        {
            BinaryWriter bw = new BinaryWriter(stream);
            List<IBlockDefinition> definitions = new List<IBlockDefinition>();

            // Types sammeln
            for (int i = 0; i < chunk.Blocks.Length; i++)
            {
                if (chunk.Blocks[i] != 0)
                {
                    IBlockDefinition definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);
                    if (!definitions.Contains(definition))
                        definitions.Add(definition);
                }
            }

            bool longIndex = definitions.Count > 254;
            bw.Write((byte)((longIndex) ? 1 : 0));

            // Schreibe Phase 1
            if (longIndex)
                bw.Write((ushort)definitions.Count);
            else
                bw.Write((byte)definitions.Count);

            // Im Falle eines Luft-Chunks...
            if (definitions.Count == 0)
                return;

            foreach (var definition in definitions)
            {
                bw.Write(definition.GetType().FullName);
            }

            // Schreibe Phase 2
            for (int i = 0; i < chunk.Blocks.Length; i++)
            {
                if (chunk.Blocks[i] == 0)
                {
                    // Definition Index (Air)
                    if (longIndex)
                        bw.Write((ushort)0);
                    else
                        bw.Write((byte)0);
                }
                else
                {
                    // Definition Index
                    IBlockDefinition definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);

                    if (longIndex)
                        bw.Write((ushort)(definitions.IndexOf(definition) + 1));
                    else
                        bw.Write((byte)(definitions.IndexOf(definition) + 1));

                    // Meta Data
                    if (definition.HasMetaData)
                        bw.Write(chunk.MetaData[i]);
                }
            }
        }
    }
}