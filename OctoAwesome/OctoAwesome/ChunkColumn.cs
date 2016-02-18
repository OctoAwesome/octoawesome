using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class ChunkColumn : IChunkColumn
    {
        public ChunkColumn(IChunk[] chunks,int planet,Index2 columnIndex) : this()
        {
            Planet = planet;
            Chunks = chunks;
            Index = columnIndex;
        }

        public ChunkColumn()
        {
            Heights = new int[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];
        }

        public void CalculateHeights()
        {
            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    Heights[x, y] = getTopBlockHeight(x, y);
                }
            }
        }

        private int getTopBlockHeight(int x, int y)
        {
            for (int z = Chunks.Length * Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
            {

                if (GetBlock(x, y, z) != 0)
                {
                    return z;
                }
            }
            return -1;
        }

        public int[,] Heights { get; private set; }

        public IChunk[] Chunks
        {
            get;
            private set;
        }

        public bool Populated
        {
            get;
            set;
        }
        public int Planet
        {
            get;
            private set;
        }
        public Index2 Index
        {
            get;
            private set;
        }
        

        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        public ushort GetBlock(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlock(x, y, z);
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockMeta(x, y, z);
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockResources(x, y, z);
        }

        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block, meta);
        }

        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlock(x, y, z,block,meta);
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockMeta(x, y, z,  meta);
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockResources(x, y, z, resources);
        }

        public void Serialize(Stream stream, IDefinitionManager definitionManager)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                // Definitionen sammeln
                List<IBlockDefinition> definitions = new List<IBlockDefinition>();
                for (int c = 0; c < Chunks.Length; c++)
                {
                    IChunk chunk = Chunks[c];
                    for (int i = 0; i < chunk.Blocks.Length; i++)
                    {
                        if (chunk.Blocks[i] != 0)
                        {
                            IBlockDefinition definition = definitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);
                            if (!definitions.Contains(definition))
                                definitions.Add(definition);
                        }
                    }
                }

                bool longIndex = definitions.Count > 254;
                bw.Write((byte)((longIndex) ? 1 : 0));

                // Schreibe Phase 1 (Column Meta: Heightmap, populated, chunkcount)
                bw.Write((byte)Chunks.Length); // Chunk Count
                bw.Write(Populated); // Populated
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        bw.Write((ushort)Heights[x, y]);

                // Schreibe Phase 2 (Block Definitionen)
                if (longIndex)
                    bw.Write((ushort)definitions.Count);
                else
                    bw.Write((byte)definitions.Count);

                foreach (var definition in definitions)
                    bw.Write(definition.GetType().FullName);

                // Schreibe Phase 3 (Chunk Infos)
                for (int c = 0; c < Chunks.Length; c++)
                {
                    IChunk chunk = Chunks[c];
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
                            IBlockDefinition definition = definitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);

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

        public void Deserialize(Stream stream, IDefinitionManager definitionManager, int planetId, Index2 columnIndex)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                // Phase 1 (Column Meta: Heightmap, populated, chunkcount)
                Chunks = new Chunk[br.ReadByte()];
                Planet = planetId;
                Index = columnIndex;

                Populated = br.ReadBoolean();
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        Heights[x, y] = br.ReadUInt16();

                // Phase 2 (Block Definitionen)
                List<IBlockDefinition> types = new List<IBlockDefinition>();
                Dictionary<ushort, ushort> map = new Dictionary<ushort, ushort>();

                bool longIndex = br.ReadByte() > 0;

                int typecount = longIndex ? br.ReadUInt16() : br.ReadByte();
                for (int i = 0; i < typecount; i++)
                {
                    string typeName = br.ReadString();
                    IBlockDefinition[] definitions = definitionManager.GetBlockDefinitions().ToArray();
                    var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                    types.Add(blockDefinition);

                    map.Add((ushort)types.Count, (ushort)(Array.IndexOf(definitions, blockDefinition) + 1));
                }

                // Phase 3 (Chunk Infos)
                for (int c = 0; c < Chunks.Length; c++)
                {
                    IChunk chunk = Chunks[c] = new Chunk(new Index3(columnIndex, c), planetId);
                    for (int i = 0; i < chunk.Blocks.Length; i++)
                    {
                        ushort typeIndex = longIndex ? br.ReadUInt16() : br.ReadByte();
                        chunk.MetaData[i] = 0;
                        if (typeIndex > 0)
                        {
                            chunk.Blocks[i] = map[typeIndex];

                            var definition = definitionManager.GetBlockDefinitionByIndex(map[typeIndex]);
                            if (definition.HasMetaData)
                                chunk.MetaData[i] = br.ReadInt32();
                        }
                    }
                }
            }
        }
    }
}
