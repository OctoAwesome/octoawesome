using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public abstract class MapPopulator : IMapPopulator
    {
        public int Order { get; protected set; }

        public abstract void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
