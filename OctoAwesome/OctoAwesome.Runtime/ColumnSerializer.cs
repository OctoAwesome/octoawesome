using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public sealed class ColumnSerializer : IColumnSerializer
    {
        public IChunkColumn Deserialize(Stream stream, int planetId, Index2 columnIndex)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                // Phase 1 (Column Meta: Heightmap, populated, chunkcount)
                Chunk[] chunks = new Chunk[br.ReadByte()];
                ChunkColumn column = new ChunkColumn(chunks, planetId, columnIndex);
                column.Populated = br.ReadBoolean();
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        column.Heights[x,y] = br.ReadUInt16();

                // Phase 2 (Block Definitionen)
                List<IBlockDefinition> types = new List<IBlockDefinition>();
                Dictionary<ushort, ushort> map = new Dictionary<ushort, ushort>();

                bool longIndex = br.ReadByte() > 0;

                int typecount = longIndex ? br.ReadUInt16() : br.ReadByte();
                for (int i = 0; i < typecount; i++)
                {
                    string typeName = br.ReadString();
                    IBlockDefinition[] definitions = DefinitionManager.GetBlockDefinitions().ToArray();
                    var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                    types.Add(blockDefinition);

                    map.Add((ushort)types.Count, (ushort)(Array.IndexOf(definitions, blockDefinition) + 1));
                }

                // Phase 3 (Chunk Infos)
                for (int c = 0; c < column.Chunks.Length; c++)
                {
                    IChunk chunk = column.Chunks[c] = new Chunk(new Index3(columnIndex, c), planetId);
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

                return column;
            }
        }

        public void Serialize(Stream stream, IChunkColumn column)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                // Definitionen sammeln
                List<IBlockDefinition> definitions = new List<IBlockDefinition>();
                for (int c = 0; c < column.Chunks.Length; c++)
                {
                    IChunk chunk = column.Chunks[c];
                    for (int i = 0; i < chunk.Blocks.Length; i++)
                    {
                        if (chunk.Blocks[i] != 0)
                        {
                            IBlockDefinition definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);
                            if (!definitions.Contains(definition))
                                definitions.Add(definition);
                        }
                    }
                }

                bool longIndex = definitions.Count > 254;
                bw.Write((byte)((longIndex) ? 1 : 0));

                // Schreibe Phase 1 (Column Meta: Heightmap, populated, chunkcount)
                bw.Write((byte)column.Chunks.Length); // Chunk Count
                bw.Write(column.Populated); // Populated
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        bw.Write((ushort)column.Heights[x, y]);

                // Schreibe Phase 2 (Block Definitionen)
                if (longIndex)
                    bw.Write((ushort)definitions.Count);
                else
                    bw.Write((byte)definitions.Count);

                foreach (var definition in definitions)
                    bw.Write(definition.GetType().FullName);

                // Schreibe Phase 3 (Chunk Infos)
                for (int c = 0; c < column.Chunks.Length; c++)
                {
                    IChunk chunk = column.Chunks[c];
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
    }
}
