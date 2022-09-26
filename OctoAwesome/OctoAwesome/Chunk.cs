using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System;
using System.Diagnostics;
using OctoAwesome.Extension;

namespace OctoAwesome
{
    /// <summary>
    /// A chunk implementation for a planet.
    /// </summary>
    public sealed class Chunk : IChunk
    {
        /// <summary>
        /// Dualistic logarithmic chunk size.
        /// Number of bits needed to address a block in chunk in x direction.
        /// </summary>
        /// <seealso cref="CHUNKSIZE_X"/>
        public const int LimitX = 4;
        /// <summary>
        /// Dualistic logarithmic chunk size.
        /// Number of bits needed to address a block in chunk in y direction.
        /// </summary>
        /// <seealso cref="CHUNKSIZE_Y"/>
        public const int LimitY = 4;
        /// <summary>
        /// Dualistic logarithmic chunk size.
        /// Number of bits needed to address a block in chunk in z direction.
        /// </summary>
        /// <seealso cref="CHUNKSIZE_Z"/>
        public const int LimitZ = 4;

        /// <summary>
        /// The size of a chunk in blocks x direction.
        /// </summary>
        /// <remarks>2 ^ <see cref="LimitX"/></remarks>
        public const int CHUNKSIZE_X = 1 << LimitX;

        /// <summary>
        /// The size of a chunk in blocks in y direction.
        /// </summary>
        /// <remarks>2 ^ <see cref="LimitY"/></remarks>
        public const int CHUNKSIZE_Y = 1 << LimitY;

        /// <summary>
        /// The size of a chunk in blocks in z direction.
        /// </summary>
        /// <remarks>2 ^ <see cref="LimitZ"/></remarks>
        public const int CHUNKSIZE_Z = 1 << LimitZ;

        /// <summary>
        /// The size of a chunk as <see cref="Index3"/> in blocks.
        /// </summary>
        public static readonly Index3 CHUNKSIZE = new Index3(CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z);
        private IChunkColumn? chunkColumnField;
        private IPlanet? planet;
        private Index3 index;

        private IChunkColumn ChunkColumn
        {
            get => NullabilityHelper.NotNullAssert(chunkColumnField, $"{nameof(ChunkColumn)} was not initialized!");
            set => chunkColumnField = NullabilityHelper.NotNullAssert(value, $"{nameof(ChunkColumn)} cannot be initialized with null!");
        }

        /// <inheritdoc />
        public ushort[] Blocks { get; }

        /// <inheritdoc />
        public int[] MetaData { get; }

        /// <inheritdoc />
        public Index3 Index
        {
            get
            {
                Debug.Assert(planet is not null, $"{nameof(IPoolElement)} was not initialized!");
                return index;
            }
            private set => index = value;
        }

        /// <inheritdoc />
        public IPlanet Planet
        {
            get => NullabilityHelper.NotNullAssert(planet, $"{nameof(IPoolElement)} was not initialized!");
            private set => planet = NullabilityHelper.NotNullAssert(value, $"{nameof(Planet)} cannot be initialized with null!");
        }

        /// <inheritdoc />
        public int Version { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chunk"/> class.
        /// </summary>
        /// <param name="pos">The position of the chunk.</param>
        /// <param name="planet">The planet the chunk is part of</param>
        public Chunk(Index3 pos, IPlanet planet)
        {
            Blocks = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            MetaData = new int[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];

            Index = pos;
            Planet = planet;
        }

        /// <inheritdoc />
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        /// <inheritdoc />
        public ushort GetBlock(int x, int y, int z)
        {
            return Blocks[GetFlatIndex(x, y, z)];
        }

        /// <inheritdoc />
        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }

        /// <inheritdoc />
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
            => SetBlock(GetFlatIndex(x, y, z), new BlockInfo(x, y, z, block, meta));

        /// <inheritdoc />
        public void SetBlock(int flatIndex, BlockInfo blockInfo)
        {
            Blocks[flatIndex] = blockInfo.Block;
            MetaData[flatIndex] = blockInfo.Meta;
            Changed?.Invoke(this);
            Version++;
            BlockChanged(blockInfo);
        }

        /// <inheritdoc />
        public void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos)
        {
            for (int i = 0; i < blockInfos.Length; i++)
            {
                var flatIndex = GetFlatIndex(blockInfos[i].Position);
                Blocks[flatIndex] = blockInfos[i].Block;
                MetaData[flatIndex] = blockInfos[i].Meta;
            }
            if (issueNotification)
            {
                Changed?.Invoke(this);

                Version++;
                BlocksChanged(blockInfos);
            }
        }

        /// <inheritdoc />
        public void FlagDirty()
        {
            Changed?.Invoke(this);
        }


        /// <inheritdoc />
        public int GetBlockMeta(int x, int y, int z)
        {
            return MetaData[GetFlatIndex(x, y, z)];
        }

        /// <inheritdoc />
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            MetaData[GetFlatIndex(x, y, z)] = meta;
            Changed?.Invoke(this);
        }

        /// <inheritdoc />
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            return Array.Empty<ushort>();
        }

        /// <inheritdoc />
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            Changed?.Invoke(this);
        }

        /// <inheritdoc />
        public void SetColumn(IChunkColumn chunkColumn)
            => ChunkColumn = chunkColumn;

        /// <inheritdoc />
        public void OnUpdate(SerializableNotification notification)
            => chunkColumnField?.OnUpdate(notification);

        /// <inheritdoc />
        public void Update(SerializableNotification notification)
        {
            if (notification is BlockChangedNotification blockChanged)
            {
                var flatIndex = GetFlatIndex(blockChanged.BlockInfo.Position);
                Blocks[flatIndex] = blockChanged.BlockInfo.Block;
                MetaData[flatIndex] = blockChanged.BlockInfo.Meta;
                Changed?.Invoke(this);
            }
            else if (notification is BlocksChangedNotification blocksChanged)
            {
                foreach (var block in blocksChanged.BlockInfos)
                {
                    var flatIndex = GetFlatIndex(block.Position);
                    Blocks[flatIndex] = block.Block;
                    MetaData[flatIndex] = block.Meta;
                }

                Changed?.Invoke(this);
            }
        }

        private void BlockChanged(BlockInfo blockInfo)
        {
            var notification = TypeContainer.Get<IPool<BlockChangedNotification>>().Rent();
            notification.BlockInfo = blockInfo;
            notification.ChunkPos = Index;
            notification.Planet = Planet.Id;

            OnUpdate(notification);

            notification.Release();
        }
        private void BlocksChanged(params BlockInfo[] blockInfos)
        {
            var notification = TypeContainer.Get<IPool<BlocksChangedNotification>>().Rent();
            notification.BlockInfos = blockInfos;
            notification.ChunkPos = Index;
            notification.Planet = Planet.Id;

            OnUpdate(notification);

            notification.Release();
        }

        /// <inheritdoc />
        public event Action<IChunk>? Changed;

        /// <summary>
        /// Calculates the flat index for accessing blocks in the <see cref="Blocks"/> array from a 3D index.
        /// </summary>
        /// <param name="x">X component of the block to get the flat index for.</param>
        /// <param name="y">Y component of the block to get the flat index for.</param>
        /// <param name="z">Z component of the block to get the flat index for.</param>
        /// <returns>The calculated flat index from the block coordinate.</returns>
        /// <remarks>If the coordinate references a block outside of chunk size it is wrapped to be local to the chunk.</remarks>
        public static int GetFlatIndex(int x, int y, int z)
        {
            return ((z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | (x & (CHUNKSIZE_X - 1));
        }
        /// <summary>
        /// Calculates the flat index for accessing blocks in the <see cref="Blocks"/> array from a 3D index.
        /// </summary>
        /// <param name="position">The block position to get the flat index for.</param>
        /// <returns>The calculated flat index from the block coordinate.</returns>
        /// <remarks>If the coordinate references a block outside of chunk size it is wrapped to be local to the chunk.</remarks>
        public static int GetFlatIndex(Index3 position)
        {
            return ((position.Z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((position.Y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | (position.X & (CHUNKSIZE_X - 1));
        }

        internal void Init(Index3 position, IPlanet planet)
        {
            Index = position;
            Planet = planet;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = 0;

            for (int i = 0; i < MetaData.Length; i++)
                MetaData[i] = 0;
        }

        /// <inheritdoc />
        public void Init(IPool pool)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Release()
        {
            Index = default;
            planet = default;
        }
    }
}
