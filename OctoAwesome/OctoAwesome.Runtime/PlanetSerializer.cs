using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public sealed class PlanetSerializer : IPlanetSerializer
    {
        public IPlanet Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IPlanet universe)
        {
            throw new NotImplementedException();
        }
    }
}
