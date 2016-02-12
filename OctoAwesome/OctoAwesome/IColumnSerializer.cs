using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IColumnSerializer
    {
        void Serialize(Stream stream, Guid universeId, int planetId, IChunkColumn column);

        IChunkColumn Deserialize(Stream stream, Guid universeId, int planetId, Index2 columnIndex);
    }
}
