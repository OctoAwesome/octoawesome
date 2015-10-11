using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ChunkSerializer : IChunkSerializer
    {
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
                    IBlockDefinition[] definitions = BlockDefinitionManager.GetBlockDefinitions().ToArray();
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

                        var definition = BlockDefinitionManager.GetForType(map[typeIndex]);
                        if (definition.HasMetaData)
                            chunk.MetaData[i] = br.ReadInt32();
                    }
                }
            }

            return chunk;
        }

        public void Serialize(Stream stream, IChunk chunk)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                List<IBlockDefinition> definitions = new List<IBlockDefinition>();

                // Types sammeln
                for (int i = 0; i < chunk.Blocks.Length; i++)
                {
                    if (chunk.Blocks[i] != 0)
                    {
                        IBlockDefinition definition = BlockDefinitionManager.GetForType(chunk.Blocks[i]);
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
                        IBlockDefinition definition = BlockDefinitionManager.GetForType(chunk.Blocks[i]);

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
}
