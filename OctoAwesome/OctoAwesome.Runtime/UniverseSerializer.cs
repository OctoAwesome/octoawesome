using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public sealed class UniverseSerializer : IUniverseSerializer
    {
        public IUniverse Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IUniverse universe)
        {
            throw new NotImplementedException();
        }
    }
}
