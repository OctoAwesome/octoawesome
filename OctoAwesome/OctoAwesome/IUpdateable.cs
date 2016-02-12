using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für Blöcke die Geupdatet werden müssen. Hat Paul für seine Bäume eingeführt.
    /// </summary>
    public interface IUpdateable
    {
        void Tick(ILocalChunkCache manager, int x, int y, int z);
    }
}
