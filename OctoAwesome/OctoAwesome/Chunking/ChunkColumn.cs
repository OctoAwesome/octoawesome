using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using OctoAwesome.Extension;

namespace OctoAwesome.Chunking
{
    /// <summary>
    /// Chunk column implementation containing <see cref="IChunk"/> in a column.
    /// </summary>
    public class ChunkColumn : IChunkColumn
    {

        /// <summary>
        /// List of all entities contained in the column.
        /// </summary>
        private readonly IEntityList entities;
        private readonly LockSemaphore entitySemaphore;

        private static ChunkPool ChunkPool
        {
            get => NullabilityHelper.NotNullAssert(chunkPool, $"{nameof(ChunkPool)} was not initialized!");
            set => chunkPool = NullabilityHelper.NotNullAssert(value, $"{nameof(ChunkPool)} cannot be initialized with null!");
        }
        private static ChunkPool? chunkPool;

        private IChunk[]? chunks;
        private IPlanet? planet;
        private IGlobalChunkCache? globalChunkCache;


        private IGlobalChunkCache GlobalChunkCache
        {
            get => NullabilityHelper.NotNullAssert(globalChunkCache, $"{nameof(GlobalChunkCache)} was not initialized!");
            set => globalChunkCache = NullabilityHelper.NotNullAssert(value, $"{nameof(GlobalChunkCache)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets the definition manager.
        /// </summary>
        public IDefinitionManager DefinitionManager { get; }

        /// <summary>
        /// Gets or sets the change counter indicating the number of changes the chunk column went through.
        /// </summary>
        /// <remarks>Used for identifying changes between frames.</remarks>
        public int ChangeCounter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkColumn"/> class.
        /// </summary>
        /// <param name="chunks">The chunks for the column.</param>
        /// <param name="planet">The planet to generate the column for.</param>
        /// <param name="columnIndex">The column position on the planet.</param>
        public ChunkColumn(IChunk[] chunks, IPlanet planet, Index2 columnIndex) : this(planet)
        {
            Chunks = chunks;
            Index = columnIndex;
            foreach (IChunk chunk in chunks)
            {
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkColumn"/> class.
        /// </summary>
        /// <remarks>This is only to be used for deserialization.</remarks>
        public ChunkColumn()
        {
            Heights = new int[Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y];
            entities = new EntityList(this);
            entitySemaphore = new LockSemaphore(1, 1);
            DefinitionManager = TypeContainer.Get<IDefinitionManager>();
            if (chunkPool == null)
                ChunkPool = TypeContainer.Get<ChunkPool>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkColumn"/> class.
        /// </summary>
        /// <param name="planet">The planet to generate the chunk column for.</param>
        public ChunkColumn(IPlanet planet)
            : this()
        {
            Planet = planet;
            GlobalChunkCache = planet.GlobalChunkCache;
        }

        private void OnChunkChanged(IChunk arg1)
        {
            ChangeCounter++;
        }

        /// <summary>
        /// Calculates the highest blocks inside the column and caches them in <see cref="Heights"/>.
        /// </summary>
        public void CalculateHeights()
        {
            for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
            {
                for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    Heights[x, y] = GetTopBlockHeight(x, y);
                }
            }
        }

        private int GetTopBlockHeight(int x, int y)
        {
            for (var z = Chunks.Length * Chunk.CHUNKSIZE_Z - 1; z >= 0; z--)
            {

                if (GetBlock(x, y, z) != 0)
                    return z;
            }
            return -1;
        }

        /// <inheritdoc />
        public int[,] Heights { get; }

        /// <inheritdoc />
        public IChunk[] Chunks
        {
            get => NullabilityHelper.NotNullAssert(chunks);
            private set => chunks = NullabilityHelper.NotNullAssert(value);
        }

        /// <inheritdoc />
        public bool Populated
        {
            get;
            set;
        }

        /// <inheritdoc />
        public IPlanet Planet
        {
            get => NullabilityHelper.NotNullAssert(planet, $"{nameof(Planet)} was not initialized!");
            private set => planet = NullabilityHelper.NotNullAssert(value, $"{nameof(Planet)} cannot be initialized with null!");
        }


        /// <inheritdoc />
        public Index2 Index
        {
            get;
            private set;
        }

        /// <inheritdoc />
        public ushort GetBlock(Index3 index) => GetBlock(index.X, index.Y, index.Z);

        /// <inheritdoc />
        public ushort GetBlock(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlock(x, y, z);
        }

        /// <inheritdoc />
        public int GetBlockMeta(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockMeta(x, y, z);
        }

        /// <inheritdoc />
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            return Chunks[index].GetBlockResources(x, y, z);
        }

        /// <inheritdoc />
        public void SetBlock(Index3 index, ushort block, int meta = 0) => SetBlock(index.X, index.Y, index.Z, block, meta);

        /// <inheritdoc />
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlock(x, y, z, block, meta);
        }

        /// <inheritdoc />
        public void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos)
        {
            foreach (var item in blockInfos.GroupBy(x => x.Position.Z / Chunk.CHUNKSIZE_Z))
            {
                Chunks[item.Key].SetBlocks(issueNotification, item.ToArray());
            }
        }

        /// <inheritdoc />
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockMeta(x, y, z, meta);
        }

        /// <inheritdoc />
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            var index = z / Chunk.CHUNKSIZE_Z;
            z %= Chunk.CHUNKSIZE_Z;
            Chunks[index].SetBlockResources(x, y, z, resources);
        }

        /// <inheritdoc />
        public void Serialize(BinaryWriter writer)
        {
            // Collect definitions
            var definitions = new List<IBlockDefinition>();
            for (var c = 0; c < Chunks.Length; c++)
            {
                IChunk chunk = Chunks[c];
                for (var i = 0; i < chunk.Blocks.Length; i++)
                {
                    if (chunk.Blocks[i] != 0)
                    {
                        var definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);
                        Debug.Assert(definition != null, nameof(definition) + " != null");
                        if (!definitions.Contains(definition))
                            definitions.Add(definition);
                    }
                }
            }

            var longIndex = definitions.Count > 254;
            writer.Write((byte)(longIndex ? 1 : 0));

            // Serialization Phase 1 (Column Meta: Heightmap, populated, chunk count)
            writer.Write((byte)Chunks.Length); // Chunk Count
            writer.Write(Populated); // Populated
            writer.Write(Index.X);
            writer.Write(Index.Y);
            writer.Write(Planet.Id);

            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    writer.Write((ushort)Heights[x, y]);

            // Serialization Phase 2 (Block definition)
            if (longIndex)
                writer.Write((ushort)definitions.Count);
            else
                writer.Write((byte)definitions.Count);

            foreach (IBlockDefinition definition in definitions)
                writer.Write(definition.GetType().FullName!);

            // Serialization Phase 3 (Chunk info)
            for (var c = 0; c < Chunks.Length; c++)
            {
                IChunk chunk = Chunks[c];
                writer.Write(chunk.Version);
                for (var i = 0; i < chunk.Blocks.Length; i++)
                {
                    if (chunk.Blocks[i] == 0)
                    {
                        // Definition index (Air)
                        if (longIndex)
                            writer.Write((ushort)0);
                        else
                            writer.Write((byte)0);
                    }
                    else
                    {
                        // Definition index
                        var definition = DefinitionManager.GetBlockDefinitionByIndex(chunk.Blocks[i]);

                        Debug.Assert(definition != null, nameof(definition) + " != null");
                        if (longIndex)
                            writer.Write((ushort)(definitions.IndexOf(definition) + 1));
                        else
                            writer.Write((byte)(definitions.IndexOf(definition) + 1));

                        // Metadata
                        if (definition.HasMetaData)
                            writer.Write(chunk.MetaData[i]);
                    }
                }
            }
            var resManager = TypeContainer.Get<IResourceManager>();
            using (var lockObj = entitySemaphore.Wait())
            {
                foreach (var entity in entities)
                    resManager.SaveComponentContainer<Entity, IEntityComponent>(entity);
            }
        }

        /// <inheritdoc />
        public void Deserialize(BinaryReader reader)
        {
            var longIndex = reader.ReadByte() > 0;

            // Phase 1 (Column Meta: Heightmap, populated, chunk count)
            Chunks = new IChunk[reader.ReadByte()]; // Chunk Count

            Populated = reader.ReadBoolean(); // Populated

            Index = new Index2(reader.ReadInt32(), reader.ReadInt32());
            int planetId = reader.ReadInt32();

            var resManager = TypeContainer.Get<IResourceManager>();
            Planet = resManager.GetPlanet(planetId);

            for (var y = 0; y < Chunk.CHUNKSIZE_Y; y++) // Heightmap
                for (var x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    Heights[x, y] = reader.ReadUInt16();

            // Phase 2 (Block definitions)
            int typecount = longIndex ? reader.ReadUInt16() : reader.ReadByte();
            var types = new List<IDefinition>();
            Span<ushort> map = stackalloc ushort[typecount];


            IDefinition[] definitions = DefinitionManager.Definitions;
            for (var i = 0; i < typecount; i++)
            {
                var typeName = reader.ReadString();
                int foundIndex = Array.FindIndex(definitions, (d) => d.GetType().FullName == typeName);
                if (foundIndex == -1)
                {
                    Debug.Assert(false, "No matching definition type found!");
                    continue;
                }
                var blockDefinition = definitions[foundIndex];
                map[types.Count] = (ushort)(foundIndex + 1);
                types.Add(blockDefinition);
            }

            // Phase 3 (Chunk Infos)
            for (var c = 0; c < Chunks.Length; c++)
            {
                IChunk chunk = Chunks[c] = ChunkPool.Rent(new Index3(Index, c), Planet);
                chunk.Version = reader.ReadInt32();
                chunk.Changed += OnChunkChanged;
                chunk.SetColumn(this);

                for (var i = 0; i < chunk.Blocks.Length; i++)
                {
                    var typeIndex = longIndex ? reader.ReadUInt16() : reader.ReadByte();
                    chunk.MetaData[i] = 0;
                    if (typeIndex > 0)
                    {
                        var definitionIndex = map[typeIndex - 1];

                        chunk.Blocks[i] = definitionIndex;

                        var definition = DefinitionManager.GetBlockDefinitionByIndex(definitionIndex);

                        if (definition is { HasMetaData: true })
                            chunk.MetaData[i] = reader.ReadInt32();
                    }
                }
            }
        }

        /// <inheritdoc />
        public void OnUpdate(SerializableNotification notification)
        {
            GlobalChunkCache.OnUpdate(notification);
        }

        /// <inheritdoc />
        public void Update(SerializableNotification notification)
        {
            if (notification is IChunkNotification chunkNotification)
            {
                Chunks
                    .FirstOrDefault(c => c.Index == chunkNotification.ChunkPos)?
                    .Update(notification);
            }
        }

        /// <inheritdoc />
        public void ForEachEntity(Action<Entity> action)
        {
            using (entitySemaphore.Wait())
            {
                foreach (var entity in entities)
                {
                    action(entity);
                }
            }
        }

        /// <inheritdoc />
        public void Add(Entity entity)
        {
            using (entitySemaphore.Wait())
                entities.Add(entity);
        }

        /// <inheritdoc />
        public void Remove(Entity entity)
        {
            using (entitySemaphore.Wait())
                entities.Remove(entity);
        }

        /// <inheritdoc />
        public IEnumerable<FailEntityChunkArgs> FailChunkEntity()
        {
            using (entitySemaphore.Wait())
                return entities.FailChunkEntity().ToList();
        }

        /// <inheritdoc />
        public void FlagDirty()
        {
            foreach (var chunk in Chunks)
            {
                chunk.FlagDirty();
            }
        }
    }
}
