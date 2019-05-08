using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Welt-Modell einer Säule aus <see cref="IChunk"/>s.
    /// </summary>
    public class ChunkColumn : IChunkColumn
    {
        private IGlobalChunkCache globalChunkCache;

        /// <summary>
        /// Auflistung aller sich in dieser Column befindenden Entitäten.
        /// </summary>
        public IEntityList Entities { get; private set; }

        public int ChangeCounter { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz einer ChunkColumn.
        /// </summary>
        /// <param name="chunks">Die Chunks für die Säule</param>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="columnIndex">Die Position der Säule</param>
        public ChunkColumn(IChunk[] chunks, int planet, Index2 columnIndex) : this()
        {
            Planet = planet;
            Chunks = chunks;
            Index = columnIndex;
            Entities = new EntityList(this);
            foreach (var chunk in chunks)
            {
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);
            }
        }

        private void OnChunkChanged(IChunk arg1, int arg2)
        {
            ChangeCounter++;
            Changed?.Invoke(this, arg1, arg2);
        }

        /// <summary>
        /// Erzeugt eine neue Instanz einer ChunkColumn.
        /// </summary>
        public ChunkColumn()
        {
            Heights = new int[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];
            Entities = new EntityList(this);
        }

        /// <summary>
        /// Errechnet die obersten Blöcke der Säule.
        /// </summary>
        public void CalculateHeights()
        {
            for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    Heights[x, y] = GetTopBlockHeight(x, y);
                }
            }
        }

        private int GetTopBlockHeight(int x, int y)
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

        /// <summary>
        /// Höhen innerhalb der Chunk-Säule (oberste Blöcke)
        /// </summary>
        public int[,] Heights { get; private set; }

        /// <summary>
        /// Die Chunks der Säule.
        /// </summary>
        public IChunk[] Chunks
        {
            get;
            private set;
        }

        /// <summary>
        /// Gibt an, ob die ChunkColumn schon von einem <see cref="IMapPopulator"/> bearbeitet wurde.
        /// </summary>
        public bool Populated
        {
            get;
            set;
        }

        /// <summary>
        /// Der Index des Planeten.
        /// </summary>
        public int Planet
        {
            get;
            private set;
        }

        /// <summary>
        /// Die Position der Säule.
        /// </summary>
        public Index2 Index
        {
            get;
            private set;
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        public ushort GetBlock(Index3 index) => GetBlock(index.X, index.Y, index.Z);

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Block-ID der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlock(x, y, z);
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockMeta(x, y, z);
        }

        /// <summary>
        /// Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockResources(x, y, z);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Position.
        /// </summary>
        /// <param name="index">Koordinate des Zielblocks innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        public void SetBlock(Index3 index, ushort block, int meta = 0) => SetBlock(index.X, index.Y, index.Z, block, meta);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        /// <param name="block">Die neue Block-ID</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlock(x, y, z, block, meta);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockMeta(x, y, z, meta);
        }

        /// <summary>
        /// Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort"/>-Array, das alle Ressourcen enthält</param>
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            int index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockResources(x, y, z, resources);
        }

        /// <summary>
        /// Serialisiert die Chunksäule in den angegebenen Stream.
        /// </summary>
        /// <param name="writer">Zielschreiber</param>
        /// <param name="definitionManager">Der verwendete DefinitionManager</param>
        public void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
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
                        IBlockDefinition definition = (IBlockDefinition)definitionManager.GetDefinitionByIndex(chunk.Blocks[i]);
                        if (!definitions.Contains(definition))
                            definitions.Add(definition);
                    }
                }
            }

            bool longIndex = definitions.Count > 254;
            writer.Write((byte)((longIndex) ? 1 : 0));

            // Schreibe Phase 1 (Column Meta: Heightmap, populated, chunkcount)
            writer.Write((byte)Chunks.Length); // Chunk Count
            writer.Write(Populated); // Populated
            writer.Write(Index.X);
            writer.Write(Index.Y);
            writer.Write(Planet);

            for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    writer.Write((ushort)Heights[x, y]);

            for (int i = 0; i < Chunks.Length; i++) // Change Counter
                writer.Write(Chunks[i].ChangeCounter);

            // Schreibe Phase 2 (Block Definitionen)
            if (longIndex)
                writer.Write((ushort)definitions.Count);
            else
                writer.Write((byte)definitions.Count);

            foreach (var definition in definitions)
                writer.Write(definition.GetType().FullName);

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
                            writer.Write((ushort)0);
                        else
                            writer.Write((byte)0);
                    }
                    else
                    {
                        // Definition Index
                        IBlockDefinition definition = (IBlockDefinition)definitionManager.GetDefinitionByIndex(chunk.Blocks[i]);

                        if (longIndex)
                            writer.Write((ushort)(definitions.IndexOf(definition) + 1));
                        else
                            writer.Write((byte)(definitions.IndexOf(definition) + 1));

                        // Meta Data
                        if (definition.HasMetaData)
                            writer.Write(chunk.MetaData[i]);
                    }
                }
            }

            //Entities schreiben
            writer.Write(Entities.Count);
            foreach (var entity in Entities)
            {
                using (MemoryStream memorystream = new MemoryStream())
                {
                    writer.Write(entity.GetType().AssemblyQualifiedName);

                    using (BinaryWriter componentbinarystream = new BinaryWriter(memorystream))
                    {
                        try
                        {
                            entity.Serialize(componentbinarystream, definitionManager);
                            writer.Write((int)memorystream.Length);
                            memorystream.WriteTo(writer.BaseStream);

                        }
                        catch (Exception)
                        {
                            writer.Write(0);
                            //throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deserialisiert die Chunksäule aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        /// <param name="definitionManager">Der verwendete DefinitionManager</param>
        /// <param name="columnIndex">Die Position der Säule</param>
        /// <param name="planetId">Der Index des Planeten</param>
        public void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            bool longIndex = reader.ReadByte() > 0;

            // Phase 1 (Column Meta: Heightmap, populated, chunkcount)
            Chunks = new Chunk[reader.ReadByte()]; // Chunk Count

            Populated = reader.ReadBoolean(); // Populated

            Index = new Index2(reader.ReadInt32(), reader.ReadInt32());
            Planet = reader.ReadInt32();

            for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    Heights[x, y] = reader.ReadUInt16();

            int[] counter = new int[Chunks.Length];

            for (int i = 0; i < Chunks.Length; i++) // ChangeCounter
                counter[i] = reader.ReadInt32();

            // Phase 2 (Block Definitionen)
            List<IDefinition> types = new List<IDefinition>();
            Dictionary<ushort, ushort> map = new Dictionary<ushort, ushort>();

            int typecount = longIndex ? reader.ReadUInt16() : reader.ReadByte();

            for (int i = 0; i < typecount; i++)
            {
                string typeName = reader.ReadString();
                IDefinition[] definitions = definitionManager.GetDefinitions().ToArray();
                var blockDefinition = definitions.FirstOrDefault(d => d.GetType().FullName == typeName);
                types.Add(blockDefinition);

                map.Add((ushort)types.Count, (ushort)(Array.IndexOf(definitions, blockDefinition) + 1));
            }

            // Phase 3 (Chunk Infos)
            for (int c = 0; c < Chunks.Length; c++)
            {
                IChunk chunk = Chunks[c] = new Chunk(new Index3(Index, c), Planet);
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);

                for (int i = 0; i < chunk.Blocks.Length; i++)
                {
                    ushort typeIndex = longIndex ? reader.ReadUInt16() : reader.ReadByte();
                    chunk.MetaData[i] = 0;
                    if (typeIndex > 0)
                    {
                        chunk.Blocks[i] = map[typeIndex];

                        var definition = (IBlockDefinition)definitionManager.GetDefinitionByIndex(map[typeIndex]);

                        if (definition.HasMetaData)
                            chunk.MetaData[i] = reader.ReadInt32();
                    }
                }
                chunk.ChangeCounter = counter[c];
            }

            //Entities lesen
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var name = reader.ReadString();
                var length = reader.ReadInt32();

                byte[] buffer = new byte[length];
                reader.Read(buffer, 0, length);

                try
                {
                    var type = Type.GetType(name);

                    if (type == null)
                        continue;

                    Entity entity = (Entity)Activator.CreateInstance(type);

                    using (MemoryStream memorystream = new MemoryStream(buffer))
                    {
                        using (BinaryReader componentbinarystream = new BinaryReader(memorystream))
                        {
                            entity.Deserialize(componentbinarystream, definitionManager);
                        }
                    }

                    Entities.Add(entity);
                }
                catch (Exception)
                {
                }
            }
        }

        public event Action<IChunkColumn, IChunk, int> Changed;

        public void SetCache(IGlobalChunkCache globalChunkCache)
            => this.globalChunkCache = globalChunkCache;

        public void OnUpdate(SerializableNotification notification)
        {
            if (notification is ChunkNotification chunkNotification)
            {
                chunkNotification.ChunkColumnIndex = Index;
                globalChunkCache.OnUpdate(notification);
            }
        }

        public void Update(SerializableNotification notification)
        {
            if (notification is ChunkNotification chunkNotification)
            {
                Chunks
                    .FirstOrDefault(c => c.Index == chunkNotification.ChunkPos)
                    .Update(notification);
            }
        }
    }
}
