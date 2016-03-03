using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Interface für das Persistieren der Welt
    /// </summary>
    public interface IPersistenceManager
    {
        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        IUniverse[] ListUniverses();

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        IUniverse LoadUniverse(Guid universeGuid);

        void SaveUniverse(IUniverse universe);

        void DeleteUniverse(Guid universeGuid);

        IPlanet LoadPlanet(Guid universeGuid, int planetId);

        /// <summary>
        /// Speichert einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planet">Zu speichernder Planet</param>
        void SavePlanet(Guid universeGuid, IPlanet planet);

        /// <summary>
        /// Lädt eine <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planet">Index des Planeten.</param>
        /// <param name="columnIndex">Zu serialisierende ChunkColumn.</param>
        /// <returns>Die neu geladene ChunkColumn.</returns>
        IChunkColumn LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex);

        void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column);

        Player LoadPlayer(Guid universeGuid, string playername);
        void SavePlayer(Guid universeGuid, Player player);
    }
}
