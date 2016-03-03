using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basisschnittstelle für die Universen in OctoAwesome. Ein Universum beinhaltet verschiedene Planeten und entspricht einem Speicherstand.
    /// </summary>
    public interface IUniverse
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

        /// <summary>
        /// Serialisiert das Usinerse in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Zielstream</param>
        void Serialize(Stream stream);

        /// <summary>
        /// Deserialisiert das Universe aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        void Deserialize(Stream stream);
    }
}
