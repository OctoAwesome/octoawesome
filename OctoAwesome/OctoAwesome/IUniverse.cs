using OctoAwesome.Serialization;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Basisschnittstelle für die Universen in OctoAwesome. Ein Universum beinhaltet verschiedene Planeten und entspricht einem Speicherstand.
    /// </summary>
    public interface IUniverse : ISerializable
    {
        /// <summary>
        /// ID des Universums
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Der Name des Universums
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Der Generierungsseed des Universums
        /// </summary>
        int Seed { get; }
    }
}
