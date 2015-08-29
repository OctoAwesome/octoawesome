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
                int typecount = br.ReadInt32();

                // Im Falle eines Luftchunks
                if (typecount == 0)
                    return chunk;

                for (int i = 0; i < typecount; i++)
                {
                    string typeName = br.ReadString();
                    IBlockDefinition[] definitions = BlockDefinitionManager.GetBlockDefinitions().ToArray();
                    var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                    types.Add(blockDefinition);

                    map.Add((ushort)(types.Count - 1), (ushort)Array.IndexOf(definitions, blockDefinition));
                }

                for (int i = 0; i < chunk.Blocks.Length; i++)
                {
                    ushort typeIndex = br.ReadUInt16();
                    chunk.MetaData[i] = br.ReadInt32();
                    if (typeIndex > 0)
                    {
                        chunk.Blocks[i] = map[typeIndex];
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

                // Schreibe Phase 1
                bw.Write(definitions.Count);

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
                        bw.Write((ushort)0);

                        // Meta Data
                        bw.Write(0);
                    }
                    else
                    {
                        // Definition Index
                        IBlockDefinition definition = BlockDefinitionManager.GetForType(chunk.Blocks[i]);
                        bw.Write((ushort)definitions.IndexOf(definition) + 1);

                        // Meta Data
                        bw.Write(chunk.MetaData[i]);
                    }
                }
            }
        }
    }
}
