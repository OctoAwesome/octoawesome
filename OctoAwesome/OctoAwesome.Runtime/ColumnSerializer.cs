using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public sealed class ColumnSerializer : IColumnSerializer
    {
        public IChunkColumn Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IChunkColumn column)
        {
            throw new NotImplementedException();
        }
    }
}
