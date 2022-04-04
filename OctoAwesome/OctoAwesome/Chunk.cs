using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System;

namespace OctoAwesome
{

    public sealed class Chunk : IChunk
    {

        public const int LimitX = 4;

        public const int LimitY = 4;

        public const int LimitZ = 4;

        public const int CHUNKSIZE_X = 1 << LimitX;

        public const int CHUNKSIZE_Y = 1 << LimitY;

        public const int CHUNKSIZE_Z = 1 << LimitZ;
        public static readonly Index3 CHUNKSIZE = new Index3(CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z);
        private IChunkColumn chunkColumn;
        public ushort[] Blocks { get; }
        public int[] MetaData { get; }
        public Index3 Index { get; private set; }
        public IPlanet Planet { get; private set; }
        public int Version { get; set; }
        public Chunk(Index3 pos, IPlanet planet)
        {
            Blocks = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            MetaData = new int[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];

            Index = pos;
            Planet = planet;
        }
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }
        public ushort GetBlock(int x, int y, int z)
        {
            return Blocks[GetFlatIndex(x, y, z)];
        }
        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
            => SetBlock(GetFlatIndex(x, y, z), new BlockInfo(x, y, z, block, meta));
        public void SetBlock(int flatIndex, BlockInfo blockInfo)
        {
            Blocks[flatIndex] = blockInfo.Block;
            MetaData[flatIndex] = blockInfo.Meta;
            Changed?.Invoke(this);
            Version++;
            BlockChanged(blockInfo);
        }
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
        public void FlagDirty()
        {
            Changed?.Invoke(this);
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            return MetaData[GetFlatIndex(x, y, z)];
        }
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            MetaData[GetFlatIndex(x, y, z)] = meta;
            Changed?.Invoke(this);
        }
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            return Array.Empty<ushort>();
        }
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            Changed?.Invoke(this);
        }
        public void SetColumn(IChunkColumn chunkColumn)
            => this.chunkColumn = chunkColumn;
        public void OnUpdate(SerializableNotification notification)
            => chunkColumn?.OnUpdate(notification);
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
        public event Action<IChunk>? Changed;

        public static int GetFlatIndex(int x, int y, int z)
        {
            return ((z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | (x & (CHUNKSIZE_X - 1));
        }
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
        public void Init(IPool pool)
        {
            throw new NotSupportedException();
        }
        public void Release()
        {
            Index = default;
            Planet = default;
        }
    }
}
