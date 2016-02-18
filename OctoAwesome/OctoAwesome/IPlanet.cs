using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basis Schnittstelle für alle Implementierungen von Planeten.
    /// </summary>
    public interface IPlanet
    {
        /// <summary>
        /// ID des Planeten.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Id des Parent Universe
        /// </summary>
        Guid Universe { get; }

        /// <summary>
        /// Seed des Zufallsgenerators dieses Planeten.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Die Größe des Planeten in Chunks.
        /// </summary>
        Index3 Size { get; }

        IClimateMap ClimateMap { get; }

        IMapGenerator Generator { get; }

        void Serialize(Stream stream);

        void Deserialize(Stream stream);
    }
}
