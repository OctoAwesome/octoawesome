using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IPlanetSerializer
    {
        void Serialize(Stream stream, Guid universeId, IPlanet universe);

        IPlanet Deserialize(Stream stream, Guid universeId, int planetId);
    }
}
