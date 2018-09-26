using System;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Basis Schnittstelle für alle Implementierungen von Planeten.
    /// </summary>
    public interface IPlanet : ISerializable
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

        /// <summary>
        /// Die Klimakarte des Planeten
        /// </summary>
        IClimateMap ClimateMap { get; }

        /// <summary>
        /// Der Generator des Planeten
        /// </summary>
        IMapGenerator Generator { get; set; }

    }
}
