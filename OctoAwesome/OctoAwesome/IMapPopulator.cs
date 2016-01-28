using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IMapPopulator
    {
        void Populate(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
