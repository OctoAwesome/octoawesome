using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IMapPopulator
    {
        /// <summary>
        /// Gibt die Rangposition des Populators an [0...99]
        /// </summary>
        int Order { get; }

        void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
