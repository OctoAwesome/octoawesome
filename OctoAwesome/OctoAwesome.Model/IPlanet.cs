using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IPlanet
    {
        int Id { get; }

        int Seed { get; }

        Index3 Size { get; }

        IChunkPersistence ChunkPersistence { get; set; }

        IChunk GetChunk(Index3 index);

        IBlock GetBlock(Index3 index);

        void SetBlock(Index3 index, IBlock block, TimeSpan time);

        void Save();
    }
}
