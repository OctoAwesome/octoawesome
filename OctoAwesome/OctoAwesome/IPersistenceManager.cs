using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für das Persistieren eines Chunks
    /// </summary>
    public interface IPersistenceManager
    {
        void SaveUniverse(IUniverse universe);

        void SavePlanet(Guid universeGuid, IPlanet planet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="universeGuid"></param>
        /// <param name="planetId"></param>
        /// <param name="column"></param>
        void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column);

        IUniverse[] ListUniverses();

        IUniverse LoadUniverse(Guid universeGuid);

        IPlanet LoadPlanet(Guid universeGuid, int planetId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="universeGuid"></param>
        /// <param name="planetId"></param>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        IChunkColumn LoadColumn(Guid universeGuid, int planetId, Index2 columnIndex);
    }
}
