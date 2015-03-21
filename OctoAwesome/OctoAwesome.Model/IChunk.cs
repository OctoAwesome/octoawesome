using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IChunk
    {
        Index3 Index { get; }

        int ChangeCounter { get; }

        IBlock GetBlock(Index3 index);

        IBlock GetBlock(int x, int y, int z);

        void SetBlock(Index3 index, IBlock block);

        void SetBlock(int x, int y, int z, IBlock block);

        void Serialize(Stream stream);

        void Deserialize(Stream stream, IEnumerable<IBlockDefinition> knownBlocks);
    }
}
