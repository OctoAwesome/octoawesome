using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IChunk
    {
        Index3 Index { get; }

        TimeSpan LastChange { get; }

        IBlock GetBlock(Index3 index);

        void SetBlock(Index3 index, IBlock block, TimeSpan time);
    }
}
