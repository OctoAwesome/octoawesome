using System;
using System.Collections.Generic;
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

        int Seed { get; }
    }
}
