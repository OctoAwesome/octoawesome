using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IColumnSerializer
    {
        void Serialize(Stream stream, IChunkColumn column);

        IChunkColumn Deserialize(Stream stream);
    }
}
