using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IUniverseSerializer
    {
        void Serialize(Stream stream, IUniverse universe);

        IUniverse Deserialize(Stream stream);
    }
}
